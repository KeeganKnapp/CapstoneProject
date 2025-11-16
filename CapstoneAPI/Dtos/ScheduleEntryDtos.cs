using System.ComponentModel.DataAnnotations;

namespace CapstoneAPI.DTOs
{
    public record ScheduleEntryCreateDto(
        [Required] int UserId,
        [Required] int? AssignmentId,
        [Required] DateOnly StartTime,
        [Required] DateOnly EndTime
    );
    public record ScheduleEntryDto(
        long ScheduleEntryId,
        int UserId,
        int? AssignmentId,
        DateOnly StartTime,
        DateOnly EndTime
    );
}