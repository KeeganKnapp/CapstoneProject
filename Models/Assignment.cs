namespace CapstoneBlazorApp.Models
{
    public class Assignment
    {
        public int AssignmentId { get; set; }
    public int JobsiteId { get; set; }
    public string Title {get; set; } = string.Empty;    public string? Description { get; set; }
    public string Status { get; set; } = "todo";
    public int TotalHours { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<UserAssignment> UserAssignments { get; set; } = new();
    public List<AssignmentComment> Comments { get; set; } = new();
}

public class UserAssignment
{
    public int AssignmentId { get; set; }
    public int UserId { get; set; }
}

public class AssignmentComment
{
    public int CommentId { get; set; }
    public int AssignmentId { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
}