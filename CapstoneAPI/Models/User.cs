/*


--------------------------------------------------------------------------------------
Defines the User entity, one record per registered account in the database

*/

namespace CapstoneAPI.Models
{
    public class User
    {
        public int UserId { get; set; }                 // "UserId"
        public string Email { get; set; } = null!;       // "Email" (citext/text)
        public string PasswordHash { get; set; } = null!;// "PasswordHash"
        public string? DisplayName { get; set; }         // "DisplayName"
        public bool IsActive { get; set; } = true;       // "IsActive"
        public DateTimeOffset CreatedAt { get; set; }    // "CreatedAt"
        public DateTimeOffset UpdatedAt { get; set; }    // "UpdatedAt"
        public string Role { get; set; } = "Employee";   // automatically hold employee role

        // navigation
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}