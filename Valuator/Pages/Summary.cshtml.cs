using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using static System.Net.Mime.MediaTypeNames;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;

    public SummaryModel(ILogger<SummaryModel> logger)
    {
        _logger = logger;
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }
    public bool IsReady { get; set; }

    public async Task<IActionResult> OnGet(string id, string region)
    {
        string? dbConnection = Environment.GetEnvironmentVariable($"DB_{region}");
        if (string.IsNullOrEmpty(dbConnection))
        {
            _logger.LogError($"No Redis configuration found for region {region}");
            return StatusCode(500, "Server configuration error.");
        }

        _logger.LogInformation($"LOOKUP: {id}, {region}");

        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(dbConnection);
        IDatabase db = redis.GetDatabase();

        string rankKey = "RANK-" + id;
        string similarityKey = "SIMILARITY-" + id;

        for (int i = 0; i < 10; i++)
        {
            if (db.KeyExists(rankKey) && db.KeyExists(similarityKey))
            {
                Rank = (double)db.StringGet(rankKey);
                Similarity = (double)db.StringGet(similarityKey);
                IsReady = true;
                break;
            }

            await Task.Delay(200);
        }

        if (!IsReady)
        {
            _logger.LogError("Values not found for ID: {id}", id);
            Rank = 0;
            Similarity = 0;
        }

        return Page();
    }
}

