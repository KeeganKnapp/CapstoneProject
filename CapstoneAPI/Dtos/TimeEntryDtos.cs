/*


--------------------------------------------------------------------------------------
Define the data structure/payload the frontend needs when an
employee clocks in or clocks out

*/

namespace CapstoneAPI.Dtos
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

    public record TimeEntryDto(
        long TimeEntryId,
        int UserId,
        int? AssignmentId,
        DateTimeOffset StartTime,
        DateTimeOffset? EndTime
    );
}