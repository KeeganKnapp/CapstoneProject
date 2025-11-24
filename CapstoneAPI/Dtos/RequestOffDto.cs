using System.ComponentModel.DataAnnotations;

namespace CapstoneAPI.DTOs
{
    public record RequestOffCreateDto(
        [Required] DateOnly StartDate,
        [Required] DateOnly EndDate,
        [MaxLength(500)] string? Note
    );

    public record RequestOffDto(
        long RequestOffId,
        int UserId,
        DateOnly StartDate,
        DateOnly EndDate,
        string? Note,
        string? Status
    );
}