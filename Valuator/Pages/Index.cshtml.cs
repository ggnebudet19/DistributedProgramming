using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NATS.Client;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IConnection _natsConnection;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        _natsConnection = new ConnectionFactory().CreateConnection();
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost(string text, string country)
    {
        if (string.IsNullOrEmpty(text))
        {
            return BadRequest("Text cannot be empty!");
        }
        string region = CountryHandler.GetRegionByCountry(country);
        string? dbConnection = Environment.GetEnvironmentVariable($"DB_{region}");
        if (string.IsNullOrEmpty(dbConnection))
        {
            _logger.LogError($"No Redis configuration found for region {region}");
            return StatusCode(500, "Server configuration error.");
        }

        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(dbConnection);
        IDatabase db = redis.GetDatabase();

        string id = Guid.NewGuid().ToString();
        string textKey = "TEXT-" + id;

        _logger.LogInformation($"LOOKUP: {id}, {region}");

        text = text.Trim();
        db.StringSet(textKey, text);

        string rankMessage = $"{id}:{text}:{region}";
        byte[] rankData = Encoding.UTF8.GetBytes(rankMessage);
        _natsConnection.Publish("valuator.processing.rank", rankData);

        int similarity = TextEvaluator.CalculateSimilarity(text, db, textKey, dbConnection);

        string similarityKey = "SIMILARITY-" + id;
        db.StringSet(similarityKey, similarity);

        string similarityMessage = $"{id}:{similarity}";
        byte[] similarityData = Encoding.UTF8.GetBytes(similarityMessage);
        _natsConnection.Publish("valuator.events.similarity", similarityData);

        return Redirect($"summary?id={id}&region={region}");
    }

}
