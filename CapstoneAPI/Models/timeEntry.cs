// represents a single time entry record from the db

using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneAPI.Models
{
    public sealed class TimeEntry
    {
        public long TimeEntryId { get; set; }    // primary key
        public int EmployeeId { get; set; }     // foreign keyy -> emplyee table
        public int? AssignmentId { get; set; }   // foreign key -> assignment (jobsite)
        public DateTime StartTime { get; set; } // UTC time employee clocked in
        public DateTime? EndTime { get; set; }  // UTC time employee clocked out (nullable)
    }
}
