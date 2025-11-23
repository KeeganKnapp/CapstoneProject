/*


--------------------------------------------------------------------------------------
Define the data structure/payload the frontend needs when an
employee clocks in or clocks out

*/

namespace CapstoneBlazorApp.Dtos
{
    // for POST /time-entries/clock-in
    public sealed class ClockInRequest
    {
        public int UserId { get; set; }
        public int? AssignmentId { get; set; }           // optional
        public DateTimeOffset? StartTime { get; set; }   // optional (defaults to UtcNow)
    }

    // for POST /time-entries/clock-out
    public sealed class ClockOutRequest
    {
        public long? TimeEntryId { get; set; }           // optional: close by id
        public int? UserId { get; set; }             // optional: close latest open by employee
        public DateTimeOffset? EndTime { get; set; }     // optional (defaults to UtcNow)
    }

    // DTO for time entry responses
    public sealed class TimeEntryDto
    {
        public long TimeEntryId { get; set; }
        public int UserId { get; set; }
        public int? AssignmentId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        
        // Calculated properties
        public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;
        public bool IsActive => !EndTime.HasValue;
    }

    // DTO for assignment responses
    public sealed class AssignmentDto
    {
        public int AssignmentId { get; set; }
        public int JobsiteId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Priority { get; set; }
        public int? AssignedUserId { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}