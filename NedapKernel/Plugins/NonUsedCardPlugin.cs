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
    public class NonUsedCardPlugin
    {
        [KernelFunction("DetectNonUsedCards")]
        public async Task<string> DetectNonUsedCardsAsync(List<eventlog> events, DateTime currentDate)
        {
            var last30Days = currentDate.AddDays(-30);
            var activeCards = events
                .Where(e => e.servertimestamp >= last30Days)
                .Select(e => e.objectid)
                .Distinct()
                .ToList();

            var allCards = events.Select(e => e.objectid).Distinct().ToList();
            var nonUsedCards = allCards.Except(activeCards).ToList();

            return JsonSerializer.Serialize(new
            {
                NonUsedCards = nonUsedCards
            });
        }
    }
}
