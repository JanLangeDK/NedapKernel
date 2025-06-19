using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using NedapKernel.Models;
using System.ComponentModel;
using System.Text;


namespace NedapKernel.Plugins
{
    public class EventLogPlugin
    {
        [KernelFunction("GetAllEventLogsInTimeFrame")]
        [Description("Lists all events within a specified timeframe.")]
        public async Task<string> GetAllEventLogsInTimeFrame(
            [Description("Start of the timeframe in ISO 8601 format (e.g., 2025-04-10T00:00:00Z)")] string startTime,
            [Description("End of the timeframe in ISO 8601 format (e.g., 2025-04-10T23:59:59Z)")] string endTime)
        {
            try
            {
                var startDateTime = DateTime.Parse(startTime).ToUniversalTime();
                var endDateTime = DateTime.Parse(endTime).ToUniversalTime();

                var eventLogInstance = new eventlog();
                List<eventlog> eventlogs = await eventLogInstance.GetEventLogsInTimeFrameAsync(
                    "Server=AZEDKNedap01;Database=aeosdb;Integrated Security=True;TrustServerCertificate=True;", startDateTime, endDateTime);

                if (eventlogs == null || eventlogs.Count == 0)
                {
                    return "No events found in the specified timeframe.";
                }
                string rtn = string.Join("\n", eventlogs.Select(e => $"{e.servertimestamp:yyyy-MM-dd HH:mm} | {e.eventtype} | {e.entranceid} | {e.carrierid}"));
                return rtn;

            }
            catch (FormatException ex)
            {
                return $"Error: Invalid date format. {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        [KernelFunction("GetCarrierEventLogsInTimeFrame")]
        [Description("Lists events for carrierid within a specified timeframe.")]
        public async Task<string> GetAllEventLogsInTimeFrame(
            [Description("Start of the timeframe in ISO 8601 format (e.g., 2025-04-10T00:00:00Z)")] string startTime,
            [Description("End of the timeframe in ISO 8601 format (e.g., 2025-04-10T23:59:59Z)")] string endTime,
            [Description("CarrierId (e.g. 60087)")] string carrierId)
        {
            try
            {
                var startDateTime = DateTime.Parse(startTime).ToUniversalTime();
                var endDateTime = DateTime.Parse(endTime).ToUniversalTime();

                var eventLogInstance = new eventlog();
                List<eventlog> eventlogs = await eventLogInstance.GetCarrierEventLogsInTimeFrameAsync(
                    "Server=AZEDKNedap01;Database=aeosdb;Integrated Security=True;TrustServerCertificate=True;", startDateTime, endDateTime, carrierId);

                if (eventlogs == null || eventlogs.Count == 0)
                {
                    return "No events found in the specified timeframe.";
                }
                string rtn = string.Join("\n", eventlogs.Select(e => $"{e.servertimestamp:yyyy-MM-dd HH:mm} | {e.eventtype} | {e.entranceid} | {e.carrierid}"));
                return rtn;

            }
            catch (FormatException ex)
            {
                return $"Error: Invalid date format. {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

    }
}