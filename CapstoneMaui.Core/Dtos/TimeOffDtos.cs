namespace CapstoneMaui.Core.Dtos;

public record TimeOffRequestDto(DateOnly start, DateOnly end, string reason);
public record TimeOffItemDto(int id, DateOnly start, DateOnly end, string status); // pending / approved / denied