/*

Bridge between database and objects
--------------------------------------------------------------------------------------
Defines which tables exist (DbSet<T>), how each C# model maps to its SQL table/columns
and the relationships between tables

*/


using Microsoft.EntityFrameworkCore;    // ef core library
using CapstoneAPI.Models;               // links entity classes


namespace CapstoneAPI.Data
{
    public class CapstoneDbContext : DbContext
    {
        // ef core creates context using options configured in Program.cs
        public CapstoneDbContext(DbContextOptions<CapstoneDbContext> options) : base(options) { }

        // each DbSet<T> represents a database table
        public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        // postgres lowercases unquoted identifiers by default so this 
        // gets the exact tables names, column names, keys, and relationships
        // so EF matches the schema, could've used snake_case
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // binds the TimeEntry class to the "TimeEntry" table, maps
            // each property to the exact column names in SQL
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

            // binds User to "Users" table, maps all properties to exact columns, 
            // configures the 1-to-many relationship (user has many refresh tokens)
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Users");

                e.HasKey(x => x.UserId);

                e.Property(x => x.UserId).HasColumnName("UserId");
                e.Property(x => x.Email).HasColumnName("Email");
                e.Property(x => x.PasswordHash).HasColumnName("PasswordHash");
                e.Property(x => x.DisplayName).HasColumnName("DisplayName");
                e.Property(x => x.IsActive).HasColumnName("IsActive");
                e.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
                e.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");

                e.HasMany(u => u.RefreshTokens).WithOne(rt => rt.User!)
                    .HasForeignKey(rt => rt.UserId);
            });

            // binds RefreshToken to "RefreshTokens", maps column exactly, sets
            // cascade delete (deleting a user automatically deletes their refresh token)
            modelBuilder.Entity<RefreshToken>(e =>
            {
                e.ToTable("RefreshTokens");

                e.HasKey(x => x.RefreshTokenId);

                e.Property(x => x.RefreshTokenId).HasColumnName("RefreshTokenId");
                e.Property(x => x.UserId).HasColumnName("UserId");
                e.Property(x => x.Token).HasColumnName("Token");
                e.Property(x => x.CreatedAt).HasColumnName("CreatedAt");
                e.Property(x => x.ExpiresAt).HasColumnName("ExpiresAt");
                e.Property(x => x.RevokedAt).HasColumnName("RevokedAt");
                e.Property(x => x.ReplacedByToken).HasColumnName("ReplacedByToken");

                e.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}