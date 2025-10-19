// acting as a bridge between C# objecs and the postgres tables


using Microsoft.EntityFrameworkCore;
using CapstoneAPI.Models;

namespace CapstoneAPI.Data
{
    public class CapstoneDbContext : DbContext
    {
        public CapstoneDbContext(DbContextOptions<CapstoneDbContext> options) : base(options) { }

        // DbSet = queryable representation of a table
        public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            // telling EF how this entity maps to the sql table
            b.Entity<TimeEntry>(e =>
            {
                e.ToTable("timeEntry");                                     // table name
                e.HasKey(x => x.TimeEntryId);                               // primary key
                e.Property(x => x.StartTime).HasColumnName("startTime");    // column name matches sql
                e.Property(x => x.EndTime).HasColumnName("endTime");        // column name matcches sql
            });
    }
    }
}
