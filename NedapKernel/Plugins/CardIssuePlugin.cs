using Microsoft.SemanticKernel;
using NedapKernel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NedapKernel.Plugins
{
    public class CardIssuePlugin
    {
        [KernelFunction("DetectCardIssues")]
        public async Task<string> DetectCardIssuesAsync(List<eventlog> events)
        {
            var cardFailures = events
                .GroupBy(e => e.identifierid)
                .Select(g => new
                {
                    CarrierId = g.FirstOrDefault(e => e.carrierid.HasValue)?.carrierid, //g.Key,
                    FailedAttempts = g.Count(e => e.eventtype == 40), // BadgeNoAccessEvent
                    TotalAttempts = g.Count()
                })
                .Where(g => g.FailedAttempts >= 2) // Threshold for issues
                .ToList();

            return JsonSerializer.Serialize(new
            {
                CardIssues = cardFailures
            });
        }
    }
}
