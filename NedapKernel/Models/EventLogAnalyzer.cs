using Microsoft.SemanticKernel;
using NedapKernel.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NedapKernel.Models
{
    public class EventLogAnalyzer
    {
        private readonly Kernel _kernel;
        private readonly CardIssuePlugin _cardIssuePlugin;
        private readonly NonUsedCardPlugin _nonUsedCardPlugin;
        private readonly FraudDetectionPlugin _fraudDetectionPlugin;
        private readonly PredictiveMaintenancePlugin _predictiveMaintenancePlugin;
        private readonly int _pageSize = 1000; 

        public EventLogAnalyzer(
            Kernel kernel,
        CardIssuePlugin cardIssuePlugin,
        NonUsedCardPlugin nonUsedCardPlugin,
        FraudDetectionPlugin fraudDetectionPlugin,
        PredictiveMaintenancePlugin predictiveMaintenancePlugin
            )
        {
            _kernel = kernel;
            _cardIssuePlugin = cardIssuePlugin;
            _nonUsedCardPlugin = nonUsedCardPlugin;
            _fraudDetectionPlugin = fraudDetectionPlugin;
            _predictiveMaintenancePlugin = predictiveMaintenancePlugin;

            // Add plugins to Kernel
            _kernel.Plugins.AddFromObject(_cardIssuePlugin);
            _kernel.Plugins.AddFromObject(_nonUsedCardPlugin);
            _kernel.Plugins.AddFromObject(_fraudDetectionPlugin);
            _kernel.Plugins.AddFromObject(_predictiveMaintenancePlugin);
        }

        public async Task<string> AnalyzeLogsAsync()
        {
            var results = new
            {
                CardIssues = new List<object>(),
                NonUsedCards = new List<int>(),
                FlaggedEvents = new List<object>(),
                MaintenancePredictions = new List<object>()
            };

            int pageNumber = 1;
            bool hasMoreData;

            do
            {
                Debug.WriteLine(pageNumber);

                eventlog eventlog = new eventlog();
                var events = await eventlog.GetEventsPageAsync("Server=AZEDKNedap01;Database=aeosdb;Integrated Security=True;TrustServerCertificate=True;", pageNumber, _pageSize);
                hasMoreData = events.Count == _pageSize;

                // Card Issues
                var cardIssuesResult = await _kernel.InvokeAsync<string>("CardIssuePlugin", "DetectCardIssues", new() { ["events"] = events });
                var cardIssuesObj = JsonSerializer.Deserialize<CardIssueResult>(cardIssuesResult);
                results.CardIssues.AddRange(cardIssuesObj.CardIssues);

                // Non-Used Cards
                // THIS CAN NOT BE USED BECAUSE ONE HAVE TO LOOK AT THE TOTAL ATTEMPTS FROM CARDISSUE OVER ALL PERIODS TO SEE IF THE CARD IS OUT OF USAGE

                //var nonUsedResult = await _kernel.InvokeAsync<string>("NonUsedCardPlugin", "DetectNonUsedCards", new()
                //{
                //    ["events"] = events,
                //    ["currentDate"] = DateTime.Now
                //});
                //var nonUsedObj = JsonSerializer.Deserialize<nonUsedCardResult>(nonUsedResult);
                //results.NonUsedCards.AddRange(nonUsedObj.NonUsedCards);

                // Fraud Attempts
                var fraudResult = await _kernel.InvokeAsync<string>("FraudDetectionPlugin", "DetectFraudAttempts", new() { ["events"] = events });
                var flaggedObj = JsonSerializer.Deserialize<FlaggedEventsResult>(fraudResult);
                results.FlaggedEvents.AddRange(flaggedObj.FlaggedEvents);

                // Predictive Maintenance
                var maintenanceResult = await _kernel.InvokeAsync<string>("PredictiveMaintenancePlugin", "PredictEquipmentMaintenance", new() { ["events"] = events });
                var predictionsObj = JsonSerializer.Deserialize<MaintenancePredictionResult>(maintenanceResult);
                results.MaintenancePredictions.AddRange(predictionsObj.MaintenancePredictions);

                pageNumber++;
                
                hasMoreData = pageNumber < 2;

            } while (hasMoreData);
            
            string returnValue = JsonSerializer.Serialize(results);

            return returnValue;
        }
    }
}
