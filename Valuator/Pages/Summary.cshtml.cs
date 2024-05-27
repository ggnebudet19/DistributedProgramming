using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Valuator.Pages;
public class SummaryModel : PageModel
{
    private readonly ILogger<SummaryModel> _logger;
    private readonly ConnectionMultiplexer _redis;

    public SummaryModel(ILogger<SummaryModel> logger)
    {
        _logger = logger;
        _redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
    }

    public double Rank { get; set; }
    public double Similarity { get; set; }
    public bool IsReady { get; set; }

    public async Task<IActionResult> OnGet(string id)
    {
        _logger.LogDebug(id);

        IDatabase db = _redis.GetDatabase();

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

