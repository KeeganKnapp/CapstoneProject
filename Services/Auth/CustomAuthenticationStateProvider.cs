using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using CapstoneBlazorApp.Services.Abstractions;
using System.IdentityModel.Tokens.Jwt;

namespace CapstoneBlazorApp.Services.Auth
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService _authService;
        private readonly ILogger<CustomAuthenticationStateProvider> _logger;
        private ClaimsPrincipal? _currentUser;

        public CustomAuthenticationStateProvider(IAuthService authService, ILogger<CustomAuthenticationStateProvider> logger)
        {
            _authService = authService;
            _logger = logger;
            
            // Subscribe to authentication state changes if the service is AuthService
            if (_authService is AuthService authServiceImpl)
            {
                authServiceImpl.AuthenticationStateChanged += OnAuthenticationStateChanged;
            }
        }
        
        private async void OnAuthenticationStateChanged(object? sender, string? token)
        {
            try
            {
                _logger.LogInformation($"Authentication state change event received. Token: {(string.IsNullOrEmpty(token) ? "null/empty" : "provided")}");
                
                if (!string.IsNullOrEmpty(token))
                {
                    await MarkUserAsAuthenticated(token);
                }
                else
                {
                    await MarkUserAsLoggedOut();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling authentication state change");
            }
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _authService.GetAuthTokenAsync();
                
                if (string.IsNullOrEmpty(token))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var claims = ParseClaimsFromJwt(token);
                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);
                _currentUser = user;

                return new AuthenticationState(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting authentication state");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            try
            {
                _logger.LogInformation($"Marking user as authenticated with token: {token?.Substring(0, Math.Min(token?.Length ?? 0, 20))}...");
                
                var claims = ParseClaimsFromJwt(token);
                var identity = new ClaimsIdentity(claims, "jwt");
                var user = new ClaimsPrincipal(identity);
                _currentUser = user;

                _logger.LogInformation($"User authenticated successfully: {user.Identity?.Name} ({user.Identity?.AuthenticationType})");
                
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking user as authenticated");
            }
        }

        public async Task MarkUserAsLoggedOut()
        {
            _logger.LogInformation("Marking user as logged out");
            _currentUser = null;
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
        }

        private List<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            
            // Handle demo tokens
            if (jwt == "demo-employee-token")
            {
                claims.Add(new Claim(ClaimTypes.Name, "Demo Employee"));
                claims.Add(new Claim(ClaimTypes.Email, "test@example.com"));
                claims.Add(new Claim(ClaimTypes.Role, "Employee"));
                claims.Add(new Claim("userId", "1"));
                _logger.LogInformation("Using demo employee token claims");
                return claims;
            }
            
            if (jwt == "manager-token")
            {
                claims.Add(new Claim(ClaimTypes.Name, "Demo Manager"));
                claims.Add(new Claim(ClaimTypes.Email, "manager@example.com"));
                claims.Add(new Claim(ClaimTypes.Role, "Manager"));
                claims.Add(new Claim("userId", "2"));
                _logger.LogInformation("Using demo manager token claims");
                return claims;
            }
            
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(jwt);

                claims.AddRange(token.Claims);
                
                // Add standard claims if they don't exist
                if (!claims.Any(c => c.Type == ClaimTypes.Name))
                {
                    var nameClaim = claims.FirstOrDefault(c => c.Type == "name") ?? 
                                   claims.FirstOrDefault(c => c.Type == "email");
                    if (nameClaim != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Name, nameClaim.Value));
                    }
                }

                if (!claims.Any(c => c.Type == ClaimTypes.Role))
                {
                    var roleClaim = claims.FirstOrDefault(c => c.Type == "role");
                    if (roleClaim != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roleClaim.Value));
                    }
                }
                
                _logger.LogInformation("Successfully parsed JWT token claims");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing JWT token, using fallback claims");
                // For demo purposes, add basic claims if JWT parsing fails
                claims.Add(new Claim(ClaimTypes.Name, "TestUser"));
                claims.Add(new Claim(ClaimTypes.Role, "Employee"));
            }

            return claims;
        }
    }
}
