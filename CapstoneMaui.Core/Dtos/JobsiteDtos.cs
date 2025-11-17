namespace CapstoneMaui.Core.Dtos;

public record AssignmentDto(int employeeId, string fullName, string role);
public record ToDoDto(string fullName, string text, bool done);