using CapstoneMaui.Core.Services.Abstractions;

namespace CapstoneMaui.Core.Services.Auth
{
    public class AuthService : IAuthService
    {
        private string? _authToken;
        private bool _isLoggedIn;

        public async Task<bool> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);

            if (email == "user@example.com" && password == "password123")
            {
                _authToken = "some-auth-token";
                _isLoggedIn = true;
                return true;
            }

            return false;
        }

        public async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            _authToken = null;
            _isLoggedIn = false;
        }

        public async Task<bool> IsUserLoggedInAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            return _isLoggedIn;
        }

        public async Task<string> GetAuthTokenAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(1000, cancellationToken);
            return _authToken;
        }
    }
}