// acting as a bridge between C# objecs and the postgres tables


using Microsoft.EntityFrameworkCore;
using CapstoneAPI.Models;

namespace CapstoneAPI.Models;


    public class CapstoneDbContext : DbContext
    {
        public CapstoneDbContext(DbContextOptions<CapstoneDbContext> options) : base(options) { }

        // DbSet = queryable representation of a table
        public DbSet<CapstoneAPI.Models.TimeEntry> TimeEntries => Set<CapstoneAPI.Models.TimeEntry>();

    }
