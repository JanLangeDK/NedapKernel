using Microsoft.EntityFrameworkCore;
using NedapKernel.Models;

namespace NedapKernel
{
    public class AppDbContext : DbContext
    {
        public DbSet<eventlog> EventLogs { get; set; }
        public DbSet<eventtype> EventTypes { get; set; }

        private readonly string _connectionString;

        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Configure table name explicitly (already done via [Table] attribute)
            modelBuilder.Entity<eventlog>().ToTable("eventlog");
            modelBuilder.Entity<eventtype>().ToTable("eventtype");
        }
    }
}
