using System.Net.Http.Headers;
using System.Text.Json;
using CapstoneBlazorApp.Dtos;
using CapstoneBlazorApp.Models;
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
                    return null;
                }
            }
            catch (Exception)
            {
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

        public async Task<bool> CreateJobsiteAsync(JobsiteDto jobsite, CancellationToken cancellationToken = default)
        {
            try
            {
                var loginResponse = await LoginAndGetTokenAsync();
                if (!string.IsNullOrEmpty(loginResponse?.AccessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
                }

                var createRequest = new
                {
                    Name = jobsite.Name,
                    Latitude = jobsite.Latitude,
                    Longitude = jobsite.Longitude,
                    RadiusMeters = jobsite.RadiusMeters
                };

                var json = JsonSerializer.Serialize(createRequest);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("/api/Jobsite", content, cancellationToken);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateJobsiteAsync(JobsiteDto jobsite, CancellationToken cancellationToken = default)
        {
            try
            {
                var loginResponse = await LoginAndGetTokenAsync();
                if (!string.IsNullOrEmpty(loginResponse?.AccessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
                }

                var updateRequest = new
                {
                    Name = jobsite.Name,
                    Latitude = jobsite.Latitude,
                    Longitude = jobsite.Longitude,
                    RadiusMeters = jobsite.RadiusMeters
                };

                var json = JsonSerializer.Serialize(updateRequest);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"/api/Jobsite/{jobsite.JobsiteId}", content, cancellationToken);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteJobsiteAsync(int jobsiteId, CancellationToken cancellationToken = default)
        {
            try
            {
                var loginResponse = await LoginAndGetTokenAsync();
                if (!string.IsNullOrEmpty(loginResponse?.AccessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
                }

                var response = await _httpClient.DeleteAsync($"/api/Jobsite/{jobsiteId}", cancellationToken);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
