/*


--------------------------------------------------------------------------------------
Defines the RefreshToken entity which represents a database table record
in "RefreshTokens"

*/

namespace CapstoneAPI.Models
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }         // "RefreshTokenId"
        public int UserId { get; set; }                 // "UserId" (FK to Users.UserId)
        public string Token { get; set; } = null!;       // "Token"
        public DateTimeOffset CreatedAt { get; set; }    // "CreatedAt"
        public DateTimeOffset ExpiresAt { get; set; }    // "ExpiresAt"
        public DateTimeOffset? RevokedAt { get; set; }   // "RevokedAt"
        public string? ReplacedByToken { get; set; }     // "ReplacedByToken"

        public User? User { get; set; }
    }
}