// represents a single time entry record from the db

using System.ComponentModel.DataAnnotations.Schema;

namespace CapstoneAPI.Models
{
    public sealed class timeEntry
    {
        public int timeEntryId { get; set; }    // primary key
        public int employeeId { get; set; }     // foreign keyy -> emplyee table
        public int assignmentId { get; set; }   // foreign key -> assignment (jobsite)
        public DateTime StartTime { get; set; } // UTC time employee clocked in
        public DateTime? EndTime { get; set; }  // UTC time employee clocked out (nullable)
    }
}
