using Microsoft.IdentityModel.Tokens;
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
    public class PredictiveMaintenancePlugin
    {
        [KernelFunction("PredictEquipmentMaintenance")]
        public async Task<string> PredictEquipmentMaintenanceAsync(List<eventlog> events)
        {
            var validEvents = events.Where(e => e.entranceid != null).ToList();

            // Group by EntranceId and count daily usage
            var usageData = validEvents
                .GroupBy(e => new { e.entranceid, Date = e.servertimestamp.Date })
                .Select(g => new
                {
                    EntranceId = g.Key.entranceid,
                    Date = g.Key.Date,
                    UsageCount = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            // Simulate ML.NET forecasting (simplified)
            var predictions = usageData
                .GroupBy(d => d.EntranceId)
                .Select(g => new
                {
                    EntranceId = g.Key,
                    PredictedMaintenanceDate = g.Max(d => d.Date).AddDays(30), // Dummy prediction
                    UsageTrend = g.Average(d => d.UsageCount)
                })
                .Where(p => p.UsageTrend > 10) // Threshold for high usage
                .ToList();

            return JsonSerializer.Serialize(new
            {
                MaintenancePredictions = predictions
            });
        }
    }
}
