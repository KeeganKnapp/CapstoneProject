using System.Net.Http.Headers;
using System.Text.Json;
using CapstoneBlazorApp.Dtos;
using CapstoneBlazorApp.Services.Abstractions;

namespace CapstoneBlazorApp.Services
{
    public class RequestOffApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public RequestOffApiService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
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

        public async Task<List<RequestOffDto>?> GetMyRequestsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                Console.WriteLine("GetMyRequestsAsync called - attempting API call");
                
                var loginResponse = await LoginAndGetTokenAsync();
                if (!string.IsNullOrEmpty(loginResponse?.AccessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
                    Console.WriteLine("JWT token obtained and set in headers");
                }

                var response = await _httpClient.GetAsync("/api/requestoff/mine", cancellationToken);
                Console.WriteLine($"API response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Response: {jsonString}");
                    
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var result = JsonSerializer.Deserialize<List<RequestOffDto>>(jsonString, options);
                    Console.WriteLine($"Deserialized {result?.Count ?? 0} requests from API");
                    
                    return result;
                }
                else
                {
                    Console.WriteLine($"API call failed with status: {response.StatusCode}, falling back to mock data");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetMyRequestsAsync: {ex.Message}, falling back to mock data");
            }
            
            var mockData = new List<RequestOffDto>
            {
                new RequestOffDto(100, 1, DateOnly.FromDateTime(DateTime.Now.AddDays(7)), DateOnly.FromDateTime(DateTime.Now.AddDays(14)), "Holiday Vacation (Mock)"),
                new RequestOffDto(101, 1, DateOnly.FromDateTime(DateTime.Now.AddDays(30)), DateOnly.FromDateTime(DateTime.Now.AddDays(32)), "Personal Time Off (Mock)"),
                new RequestOffDto(102, 1, DateOnly.FromDateTime(DateTime.Now.AddDays(60)), DateOnly.FromDateTime(DateTime.Now.AddDays(63)), "Medical Appointment (Mock)")
            };
            
            Console.WriteLine($"Returning {mockData.Count} mock requests as fallback");
            return mockData;
        }

        public async Task<RequestOffDto?> CreateRequestAsync(RequestOffCreateDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var loginResponse = await LoginAndGetTokenAsync();
                if (!string.IsNullOrEmpty(loginResponse?.AccessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
                }

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync("/api/requestoff", content, cancellationToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<RequestOffDto>(responseJson, options);
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> DeleteRequestAsync(long requestId, CancellationToken cancellationToken = default)
        {
            try
            {
                var loginResponse = await LoginAndGetTokenAsync();
                if (!string.IsNullOrEmpty(loginResponse?.AccessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
                }

                var response = await _httpClient.DeleteAsync($"/api/requestoff/{requestId}", cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
