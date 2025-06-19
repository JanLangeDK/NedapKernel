using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NedapKernel.Models
{
    public class eventlog
    {
        //[Key]
        public DateTime? timestamp { get; set; }
        public DateTime servertimestamp { get; set; }
        public long? eventtype { get; set; }
        public long? accesspointid { get; set; }
        public long? entranceid { get; set; }
        public long? identifierid { get; set; }
        public long? carrierid { get; set; }
        public int? intvalue { get; set; }
        public string? hostname { get; set; }
        public string? attributes { get; set; }
        [Key]
        public long objectid { get; set; }

        public async Task<List<eventlog>> GetEventLogsInTimeFrameAsync(string connectionString, DateTime starttime, DateTime endtime)
        {
            using (var context = new AppDbContext(connectionString))
            {
                return await context.EventLogs
                    .Where(e => e.servertimestamp >= starttime && e.servertimestamp <= endtime)
                    .ToListAsync();
            }
        }

        public async Task<List<eventlog>> GetCarrierEventLogsInTimeFrameAsync(string connectionString, DateTime starttime, DateTime endtime, string carrierId)
        {
            using (var context = new AppDbContext(connectionString))
            {
                return await context.EventLogs
                    .Where(e => e.servertimestamp >= starttime && e.servertimestamp <= endtime && carrierId == carrierId)
                    .ToListAsync();
            }
        }

        


        public async Task<List<eventlog>> GetEventsPageAsync(string connectionString, int pageNumber, int pageSize)
        {
            using (var context = new AppDbContext(connectionString))
            {
                int offset = (pageNumber - 1) * pageSize;
                //return await context.EventLogs.Skip(offset).Take(pageSize).ToListAsync();

                return await context.EventLogs
                    .Where(e => e.carrierid != null && e.carrierid != 0) // Filter out null or invalid CardId
                    .OrderBy(e => e.servertimestamp) // Ensure consistent ordering
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

            }
        }

    }
}