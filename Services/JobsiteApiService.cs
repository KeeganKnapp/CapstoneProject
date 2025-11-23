using System.Net.Http.Headers;
using System.Text.Json;
using CapstoneBlazorApp.Dtos;
using CapstoneBlazorApp.Services.Abstractions;

namespace CapstoneBlazorApp.Services
{
    public class JobsiteApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public JobsiteApiService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            // For now, we'll skip authentication headers since the demo auth doesn't provide JWT tokens
            // This can be updated when proper JWT authentication is implemented
            return;
        }

        public async Task<List<JobsiteResponse>?> GetAllJobsitesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // First try to get a JWT token by logging in
                var loginResponse = await LoginAndGetTokenAsync();
                if (!string.IsNullOrEmpty(loginResponse?.AccessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
                }
                
                var response = await _httpClient.GetAsync("/api/Jobsite", cancellationToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<List<JobsiteResponse>>(jsonString, options);
                }
                else
                {
                    // If unauthorized, return null to trigger fallback
                    return null;
                }
            }
            catch (Exception)
            {
                // Return null to trigger fallback data
                return null;
            }
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

        public async Task<JobsiteResponse?> GetJobsiteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.GetAsync($"/api/Jobsite/{id}", cancellationToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<JobsiteResponse>(jsonString, options);
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
