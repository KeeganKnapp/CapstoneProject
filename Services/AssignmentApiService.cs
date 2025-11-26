using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CapstoneBlazorApp.Dtos;
using CapstoneBlazorApp.Services.Abstractions;

namespace CapstoneBlazorApp.Services
{
    public class AssignmentApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public AssignmentApiService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        private async Task<AuthResponse?> LoginAndGetTokenAsync()
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Email = "admin@example.com", // Demo credentials
                    Password = "password123"
                };

                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/auth/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<AuthResponse>(jsonString, options);
                }
            }
            catch (Exception)
            {
                // login failed
            }
            return null;
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var loginResponse = await LoginAndGetTokenAsync();
            if (!string.IsNullOrEmpty(loginResponse?.AccessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
            }
        }

        public async Task<List<AssignmentResponse>?> GetAssignmentsByJobsiteAsync(int jobsiteId, CancellationToken cancellationToken = default)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.GetAsync($"/api/Assignment/jobsite/{jobsiteId}", cancellationToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<AssignmentResponse>>(jsonString, options);
                }
                else
                {
                    Console.WriteLine($"Failed to get assignments by jobsite: {response.StatusCode}");
                    return new List<AssignmentResponse>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAssignmentsByJobsiteAsync: {ex.Message}");
                return new List<AssignmentResponse>();
            }
        }

        public async Task<List<AssignmentResponse>> GetAllAssignmentsAsync(List<int> jobsiteIds, CancellationToken cancellationToken = default)
        {
            var allAssignments = new List<AssignmentResponse>();

            try
            {
                await SetAuthorizationHeaderAsync();

                // Get assignments for each jobsite and combine them
                foreach (var jobsiteId in jobsiteIds)
                {
                    var jobsiteAssignments = await GetAssignmentsByJobsiteAsync(jobsiteId, cancellationToken);
                    if (jobsiteAssignments != null)
                    {
                        allAssignments.AddRange(jobsiteAssignments);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAllAssignmentsAsync: {ex.Message}");
            }

            return allAssignments;
        }

        public async Task<AssignmentResponse?> GetAssignmentByIdAsync(int assignmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.GetAsync($"/api/Assignment/{assignmentId}", cancellationToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<AssignmentResponse>(jsonString, options);
                }
                else
                {
                    Console.WriteLine($"Failed to get assignment by ID: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAssignmentByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateAssignmentAsync(AssignmentCreateRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Assignment", content, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in CreateAssignmentAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<UserResponse>?> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var response = await _httpClient.GetAsync("/auth/allusers", cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<UserResponse>>(jsonString, options);
                }
                else
                {
                    Console.WriteLine($"Failed to get all users: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetAllUsersAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateAssignmentAsync(int assignmentId, AssignmentUpdateRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/Assignment/{assignmentId}", content, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in UpdateAssignmentAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAssignmentAsync(int assignmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                await SetAuthorizationHeaderAsync();
                
                var response = await _httpClient.DeleteAsync($"/api/Assignment/{assignmentId}", cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in DeleteAssignmentAsync: {ex.Message}");
                return false;
            }
        }
    }
}
