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

            if (email == "test" && password == "test")
            {
                //return employee token for demo purposes
                _authToken = "employee-token";
                _isLoggedIn = true;
                return true;
            }

            return false;
        }

        //check if user is manager for navigation purposes
        public async Task<bool> IsUserManagerAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(500, cancellationToken);
            //for demo purposes, if the auth token is "manager-token", return true
            return _authToken == "manager-token";
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