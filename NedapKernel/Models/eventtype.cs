using Microsoft.EntityFrameworkCore;
using NedapKernel;
using System.ComponentModel.DataAnnotations;

public class eventtype
{
    public string name { get; set; }
    public string classname { get; set; }
    public int? alarm { get; set; }
    public int? priority { get; set; }
    [Key]
    public long objectid { get; set; }
    public string servergenerated { get; set; }
    public int eventcategory { get; set; }

    public async Task<List<eventtype>> GetEventTypesAsync(string connectionString)
    {
        using (var context = new AppDbContext(connectionString))
        {
            return await context.EventTypes
                .ToListAsync();
        }
    }
}