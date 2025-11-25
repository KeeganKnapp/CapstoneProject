namespace CapstoneAPI.Dtos
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; } = "Employee"; // "Employee" or "Manager"
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}