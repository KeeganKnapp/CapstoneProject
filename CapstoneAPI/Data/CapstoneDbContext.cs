// acting as a bridge between C# objecs and the postgres tables


using Microsoft.EntityFrameworkCore;
using CapstoneAPI.Models;
using System.Dynamic;

namespace CapstoneAPI.Data
{
    public class CapstoneDbContext : DbContext
    {
        public CapstoneDbContext(DbContextOptions<CapstoneDbContext> options) : base(options) { }

        public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entitity<TimeEntry>(entity =>
            {
                entity.ToTable("TimeEntry");
                entity.HasKey(entity => entity.Id);
            });
        }
    }
}