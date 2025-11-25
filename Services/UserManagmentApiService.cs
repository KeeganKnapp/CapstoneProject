using System.Net.Http.Headers;
using System.Text.Json;
using CapstoneBlazorApp.Dtos;
using CapstoneBlazorApp.Services.Abstractions;

namespace CapstoneBlazorApp.Services
{
    public class UserManagementApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly AbstractLoggerService _logger;

        public UserManagementApiService(HttpClient httpClient, IAuthService authService, AbstractLoggerService logger)
        {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;
        }

        private async Task<AuthResponse?> LoginAndGetTokenAsync()
        {
            try
            {
                var loginRequest = new
                {
                    email = "test",
                    password = "test"
                };

                var loginJson = JsonSerializer.Serialize(loginRequest);
                var loginContent = new StringContent(loginJson, System.Text.Encoding.UTF8, "application/json");
                
                var loginResponse = await _httpClient.PostAsync("/auth/login", loginContent);
                
                if (loginResponse.IsSuccessStatusCode)
                {
                    var responseJson = await loginResponse.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<AuthResponse>(responseJson, options);
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var token = await _authService.GetAuthTokenAsync();
                if (token == null) return false;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.DeleteAsync($"/auth/users/{userId}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                _logger.Log(this, $"Failed to delete user with ID {userId}.", "error");
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(int userId, UserChangeRequest updatedUser)
        {
            try
            {
                var token = await _authService.GetAuthTokenAsync();
                if (token == null) return false;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var userJson = JsonSerializer.Serialize(updatedUser);
                var content = new StringContent(userJson, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/auth/users/{userId}", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                _logger.Log(this, $"Failed to update user with ID {userId}.", "error");
                return false;
            }
        }


        public async Task<List<UserResponse>?> GetAllUsersAsync()
        {
            try
            {
                var token = await _authService.GetAuthTokenAsync();
                if (token == null) return null;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("auth/allusers");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<UserResponse>>(jsonString, options);
                }
                
                return null;
            }
            catch (Exception)
            {
                _logger.Log(this, "Error fetching all users.", "error");
                return null;
            }
        }

        public async Task CreateUserAsync(RegisterRequest newUser)
        {
            try
            {
                var token = await _authService.GetAuthTokenAsync();
                if (token == null) return;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var userJson = JsonSerializer.Serialize(newUser);
                var content = new StringContent(userJson, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/auth/register", content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                _logger.Log(this, "Failed to create new user.", "error");
            }
        }
    }
}

