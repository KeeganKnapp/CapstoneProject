// represents a single time entry record from the db

using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneAPI.Models
{
    public sealed class ScheduleEntry
    {
        public long ScheduleEntryId { get; set; }    // primary key
        public int UserId { get; set; }     // foreign keyy -> users table
        public int? AssignmentId { get; set; }   // foreign key -> assignment (jobsite)
        public DateOnly StartTime { get; set; } // UTC time employee clocked in
        public DateOnly EndTime { get; set; }  // UTC time employee clocked out (nullable)
    }
}
