// acting as a bridge between C# objecs and the postgres tables


using Microsoft.EntityFrameworkCore;
using CapstoneAPI.Models;
using System.Security.Cryptography.X509Certificates;

namespace CapstoneAPI.Data
{
    public class CapstoneDbContext : DbContext
    {
        public CapstoneDbContext(DbContextOptions<CapstoneDbContext> options) : base(options) { }

        public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TimeEntry>(e =>
            {
                e.ToTable("TimeEntry");
                e.HasKey(x => x.TimeEntryId);
                e.Property(x => x.TimeEntryId).HasColumnName("TimeEntryId");
                e.Property(x => x.EmployeeId).HasColumnName("EmployeeId");
                e.Property(x => x.AssignmentId).HasColumnName("AssignmentId");
                e.Property(x => x.StartTime).HasColumnName("StartTime");
                e.Property(x => x.EndTime).HasColumnName("EndTime");
            });
        }
    }
}