using CapstoneBlazorApp.Services.Abstractions;
using CapstoneBlazorApp.Dtos;
using System.Net.Http.Json;

namespace CapstoneBlazorApp.Services.Auth
{
    public class AuthService : IAuthService
    {
        private string? _authToken;
        private string? _role;
        private readonly HttpClient _httpClient;
        
        // Event to notify when authentication state changes
        public event EventHandler<string?>? AuthenticationStateChanged;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Email = email,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync("auth/login", loginRequest, cancellationToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken);                    if (authResponse?.AccessToken != null)
                    {
                        _authToken = authResponse.AccessToken;
                        _role = authResponse.Role;
                        // Update HTTP client with new token
                        _httpClient.DefaultRequestHeaders.Authorization = 
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);
                        
                        // Notify authentication state change
                        AuthenticationStateChanged?.Invoke(this, _authToken);
                        
                        return true;
                    }
                }                Console.WriteLine($"Login API failed with status: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                Console.WriteLine($"Login API error: {errorContent}");

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }        //check if user is manager for navigation purposes
        public async Task<bool> IsUserManagerAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(500, cancellationToken);
            //return true if user's role is manager
            return _role == "Manager";
        }        public async Task LogoutAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _authToken = null;
                
                // Clear authorization header
                _httpClient.DefaultRequestHeaders.Authorization = null;
                
                // Notify authentication state change
                AuthenticationStateChanged?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");
            }
        }

        public async Task<bool> IsUserLoggedInAsync(CancellationToken cancellationToken = default)
        {
            return !string.IsNullOrEmpty(_authToken);
        }

        public async Task<string?> GetAuthTokenAsync(CancellationToken cancellationToken = default)
        {
            return _authToken;
        }
    }
}
