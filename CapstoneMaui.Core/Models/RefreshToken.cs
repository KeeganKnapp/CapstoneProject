/*


--------------------------------------------------------------------------------------
Defines the RefreshToken entity which represents a database table record
in "RefreshTokens"

*/

namespace CapstoneMaui.Core.Models
{
    public class RefreshToken
    {
        public Guid RefreshTokenId { get; set; }         // "RefreshTokenId"
        public Guid UserId { get; set; }                 // "UserId" (FK to Users.UserId)
        public string Token { get; set; } = null!;       // "Token"
        public DateTimeOffset CreatedAt { get; set; }    // "CreatedAt"
        public DateTimeOffset ExpiresAt { get; set; }    // "ExpiresAt"
        public DateTimeOffset? RevokedAt { get; set; }   // "RevokedAt"
        public string? ReplacedByToken { get; set; }     // "ReplacedByToken"

        public User? User { get; set; }
    }
}