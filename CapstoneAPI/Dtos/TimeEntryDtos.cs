namespace CapstoneAPI.Dtos
{
    // For POST /time-entries/clock-in
    public sealed class ClockInRequest
    {
        public int EmployeeId { get; set; }              // required (>0)
        public int? AssignmentId { get; set; }           // optional
        public DateTimeOffset? StartTime { get; set; }   // optional (defaults to UtcNow)
    }

    // For POST /time-entries/clock-out
    public sealed class ClockOutRequest
    {
        public long? TimeEntryId { get; set; }           // optional: close by id
        public int? EmployeeId { get; set; }             // optional: close latest open by employee
        public DateTimeOffset? EndTime { get; set; }     // optional (defaults to UtcNow)
    }
}