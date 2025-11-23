namespace CapstoneBlazorApp.Dtos
{
    public class AssignmentCreateRequest
    {
        public int JobsiteId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<int>? AssignedUserIds { get; set; } // if manager wants to assign initially
    }

    public class AssignmentUpdateRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<int>? AssignedUserIds { get; set; } // can replace all assigned users but can be ignored and keep exisiting users if null
    }

    public class AssignmentResponse
    {
        public int AssignmentId { get; set; }
        public int JobsiteId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = "todo"; // "todo", "in_progress", or "done"
        public int TotalHours { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<int> AssignedUserIds { get; set; } = new(); // users currently assigned to this assignmend
    }

    public class AssignmentCommentCreateRequest
    {
        public string Text { get; set; } = string.Empty;
    }

    public class AssignmentCommentResponse
    {
        public int CommentId { get; set; }
        public int AssignmentId { get; set; }
        public int UserId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}