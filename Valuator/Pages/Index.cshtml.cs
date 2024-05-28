using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NATS.Client;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ConnectionMultiplexer _redis;
    private readonly IConnection _natsConnection;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
        _redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        _natsConnection = new ConnectionFactory().CreateConnection();
    }

    public void OnGet()
    {

    }

    public IActionResult OnPost(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return BadRequest("Text cannot be empty!");
        }
        text = text.Trim();
        _logger.LogDebug(text);

        string id = Guid.NewGuid().ToString();
        string textKey = "TEXT-" + id;
        IDatabase db = _redis.GetDatabase();
        db.StringSet(textKey, text);

        string message = $"{id}:{textKey}";
        byte[] data = Encoding.UTF8.GetBytes(message);
        _natsConnection.Publish("valuator.processing.rank", data);
        _logger.LogInformation($"Message sent to NATS: {message}");

        string similarityKey = "SIMILARITY-" + id;
        int similarity = TextEvaluator.CalculateSimilarity(text, db, textKey);
        db.StringSet(similarityKey, similarity);

        return Redirect($"summary?id={id}");
    }
}
