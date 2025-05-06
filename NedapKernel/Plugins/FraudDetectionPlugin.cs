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
    public class FraudDetectionPlugin
    {
        private readonly Kernel _kernel;

        public FraudDetectionPlugin(Kernel kernel)
        {
            _kernel = kernel;
        }

        [KernelFunction("DetectFraudAttempts")]
        public async Task<string> DetectFraudAttemptsAsync(List<eventlog> events)
        {
            var suspiciousEvents = events
                .Where(e => e.servertimestamp.Hour < 7 || e.servertimestamp.Hour >= 18 && e.eventtype == 40) // BadgeNoAccessEvent
                .ToList();

            var prompt = @"Analyze the following access events for potential fraud. Flag events outside 9 AM–5 PM or with multiple 
            failed attempts as suspicious. Provide a JSON response with flagged events and reasons.  Please dont write any introduction or 
            any ending comments because i need to process the json so only respond with json, avoid starting and trailing ```json 

            Events:
            {events}

            Format:
            {
                ""FlaggedEvents"": [
                    { ""carrierid"": ""string"", ""servertimestamp"": ""string"", ""entranceid"": ""string"", ""reason"": ""string"" }
                ]
            }
            ";

            var eventsJson = JsonSerializer.Serialize(suspiciousEvents);
            var result = await _kernel.InvokePromptAsync(prompt.Replace("{events}", eventsJson));

            return result.ToString();
        }
    }
}
