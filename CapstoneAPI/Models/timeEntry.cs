// represents a single time entry record from the db

using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneAPI.Models
{
    public sealed class TimeEntry
    {
        public long TimeEntryId { get; set; }    // primary key
        public int UserId { get; set; }     // foreign keyy -> users table
        public int? AssignmentId { get; set; }   // foreign key -> assignment (jobsite)
        public DateTimeOffset StartTime { get; set; } // UTC time employee clocked in
        public DateTimeOffset? EndTime { get; set; }  // UTC time employee clocked out (nullable)
    }
}
