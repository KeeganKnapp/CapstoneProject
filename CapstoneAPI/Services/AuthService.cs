/*

Auth flow core
--------------------------------------------------------------------------------------
Implements the authentication use-cases (register, login, refresh, logout, logout all)
it talks to the database via CapstoneDbContext _db, token minting via ITokenService _tokens
and hashing helper via PAsswordHasher

Controllers call this service and returns DTOs (like AuthResponse) or throws exception
that controllers convert to HTTP codes

*/

using CapstoneAPI.Data;
using CapstoneAPI.Dtos;
using CapstoneAPI.Helpers;
using CapstoneAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly CapstoneDbContext _db; // ef core DbContext for Users/RefreshTokens
        private readonly ITokenService _tokens; // creates access and refresh tokens
        private readonly string[] _managerEmails;

        public AuthService(CapstoneDbContext db, ITokenService tokens, string[] managerEmails)
        {
            _db = db;
            _tokens = tokens;
            _managerEmails = managerEmails;
        }

        // create a new user, hash their password, and issue access and refresh tokens
        public async Task<AuthResponse> RegisterAsync(RegisterRequest req, CancellationToken ct)
        {
            // store email as typed (emailRaw) but compare case-insensitive using emailNorm
            var emailRaw = (req.Email ?? string.Empty).Trim();
            var emailNorm = emailRaw.ToLowerInvariant(); // for comparisons only
            if (string.IsNullOrWhiteSpace(emailRaw))
                throw new InvalidOperationException("Email required.");
            if (string.IsNullOrWhiteSpace(req.Password))
                throw new InvalidOperationException("Password required.");

            // uniqueness check (case-insensitive)
            var exists = await _db.Users.AnyAsync(u => u.Email.ToLower() == emailNorm, ct);
            if (exists) throw new InvalidOperationException("Email already registered.");

            
            //var role = _managerEmails.Contains(emailRaw, StringComparer.OrdinalIgnoreCase) ? "Manager" : "Employee";
            var role = req.Role == "Manager" ? "Manager" : "Employee";
            // creates a new user
            var user = new User
            {
                Email = emailRaw,
                DisplayName = string.IsNullOrWhiteSpace(req.DisplayName) ? null : req.DisplayName!.Trim(),
                PasswordHash = PasswordHasher.Hash(req.Password),  // PBKDF2
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                Role = role
            };

            _db.Users.Add(user);

            // if the 2 requests hit the same email at once, "already registered" is returned
            try
            {
                await _db.SaveChangesAsync(ct);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException?.Message.Contains("unique", StringComparison.OrdinalIgnoreCase) == true)
                    throw new InvalidOperationException("Email already registered.");
                throw;
            }

            // issue tokens
            var (access, accessExp) = _tokens.CreateAccessToken(user);
            var (refresh, refreshExp) = _tokens.CreateRefreshToken();

            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.UserId,
                Token = refresh,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = refreshExp
            });
            await _db.SaveChangesAsync(ct);

            // return payload the client will store
            // controller maps 400 Bad Request ("email required", "email already registered")
            return new AuthResponse
            {
                AccessToken = access,
                AccessTokenExpiresAt = accessExp,
                RefreshToken = refresh,
                RefreshTokenExpiresAt = refreshExp,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Role = user.Role
            };
        }
        public async Task DeleteUserAsync(int userId, CancellationToken ct) 
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId, ct);

            if (user == null)
                throw new KeyNotFoundException($"User {userId} not found.");

            _db.Users.Remove(user);
            await _db.SaveChangesAsync(ct);
        } 


        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync(CancellationToken ct)
        {
            return await _db.Users.Select(u => new UserResponse
            {
                UserId = u.UserId,
                Email = u.Email,
                DisplayName = u.DisplayName,
                Role = u.Role,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            }).ToListAsync(ct);
        }

        public async Task UpdateUserAsync(int userId, UserChangeRequest req, CancellationToken ct)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId, ct);

            if (user == null)
                throw new KeyNotFoundException($"User {userId} not found.");

            if (!string.IsNullOrWhiteSpace(req.Name))
                user.DisplayName = req.Name.Trim();

            if (!string.IsNullOrWhiteSpace(req.Email))
            {
                var emailNorm = req.Email.Trim().ToLowerInvariant();
                var exists = await _db.Users.AnyAsync(u => u.Email.ToLower() == emailNorm && u.UserId != userId, ct);
                if (exists)
                    throw new InvalidOperationException("Email already registered.");

                user.Email = req.Email.Trim();
            }

            if (!string.IsNullOrWhiteSpace(req.Password))
                user.PasswordHash = PasswordHasher.Hash(req.Password);

            user.UpdatedAt = DateTimeOffset.UtcNow;

            await _db.SaveChangesAsync(ct);
        }


        // authenticate credentials and issue tokens
        public async Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct)
        {
            // normalize input like registration
            var emailRaw = (req.Email ?? string.Empty).Trim();
            var emailNorm = emailRaw.ToLowerInvariant();

            // case-insensitive email lookup
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == emailNorm, ct);

            // if user is not found return Unauthorized (401)
            if (user is null)
                throw new UnauthorizedAccessException("Invalid credentials.");
            if (!user.IsActive)
                throw new UnauthorizedAccessException("Account disabled.");

            // verify PBKDF2 password by re deriving the has with the same salt and comparing
            if (!PasswordHasher.Verify(req.Password ?? string.Empty, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials.");

            // if password is valid, generates new tokens
            var (access, accessExp) = _tokens.CreateAccessToken(user);
            var (refresh, refreshExp) = _tokens.CreateRefreshToken();

            // stores new refresh tokens in the db
            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.UserId,
                Token = refresh,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = refreshExp
            });
            await _db.SaveChangesAsync(ct);

            // returns tokens and user info
            return new AuthResponse
            {
                AccessToken = access,
                AccessTokenExpiresAt = accessExp,
                RefreshToken = refresh,
                RefreshTokenExpiresAt = refreshExp,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Role = user.Role
            };
        }


        // handles refreshing an expired acces token using a valid refresh token
        // verifies the refresh token, rotates it, and issues a new pair
        public async Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken ct)
        {
            // load refresh token and associated user
            var token = await _db.RefreshTokens
                                .Include(t => t.User)
                                .FirstOrDefaultAsync(t => t.Token == refreshToken, ct);

            // validate tokens integrity
            if (token is null)
                throw new UnauthorizedAccessException("Invalid refresh token.");
            if (token.RevokedAt is not null)
                throw new UnauthorizedAccessException("Refresh token revoked.");
            if (token.ExpiresAt <= DateTimeOffset.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expired.");
            if (token.User is null || !token.User.IsActive)
                throw new UnauthorizedAccessException("Account disabled.");

            var user = token.User;

            // rotate: mark current token as revoked and issue a new one
            token.RevokedAt = DateTimeOffset.UtcNow;
            var (newRefresh, newRefreshExp) = _tokens.CreateRefreshToken();
            token.ReplacedByToken = newRefresh;
            
            // save new refresh token as a new db row
            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.UserId,
                Token = newRefresh,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiresAt = newRefreshExp
            });

            // new access token
            var (access, accessExp) = _tokens.CreateAccessToken(user);
            await _db.SaveChangesAsync(ct);

            // returns updated tokens to client
            return new AuthResponse
            {
                AccessToken = access,
                AccessTokenExpiresAt = accessExp,
                RefreshToken = newRefresh,
                RefreshTokenExpiresAt = newRefreshExp,
                Email = user.Email,
                DisplayName = user.DisplayName,
                Role = user.Role
            };
        }

        // revokes a single refresh token 
        public async Task LogoutAsync(string refreshToken, CancellationToken ct)
        {
            // look for the specific refresh token in the db
            var token = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken, ct);
            if (token is null) return; // if it doesnt exist do nothing

            // mark token as revoked
            token.RevokedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        // revokes all refresh tokens for a user, loggin them out of all devices
        public async Task LogoutAllAsync(int userId, CancellationToken ct)
        {
            // select all active (non-revoked) refresh tokens for this user
            var active = _db.RefreshTokens.Where(t => t.UserId == userId && t.RevokedAt == null);
            
            // update all of them
            await active.ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, DateTimeOffset.UtcNow), ct);
        }
    }
}