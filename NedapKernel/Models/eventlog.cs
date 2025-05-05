using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NedapKernel.Models
{
    public class eventlog
    {
        //[Key]
        public DateTime? timestamp { get; set; }
        public DateTime? servertimestamp { get; set; }
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

        public async Task<List<eventlog>> GetEventLogsAsync(string connectionString, int days = 30)
        {
            using (var context = new AppDbContext(connectionString))
            {
                var thresholdDate = DateTime.Now.AddDays(-days);
                return await context.EventLogs
                    .Where(e => e.timestamp >= thresholdDate)
                    .ToListAsync();
            }
        }
    }
}
