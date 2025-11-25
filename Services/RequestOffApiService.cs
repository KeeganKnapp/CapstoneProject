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

        public async Task<List<RequestOffDto>?> GetRequestsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                Console.WriteLine("GetMyRequestsAsync called - attempting API call");
                 
                var loginResponse = await LoginAndGetTokenAsync();
                if (!string.IsNullOrEmpty(loginResponse?.AccessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
                }

                var response = await _httpClient.GetAsync($"/api/requestoff/{userId}", cancellationToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<List<RequestOffDto>>(jsonString, options);
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


        public async Task<List<RequestOffDto>?> GetAllRequestsAsync(CancellationToken cancellationToken = default) {
            try
            {
                var loginResponse = await LoginAndGetTokenAsync();
                if (!string.IsNullOrEmpty(loginResponse?.AccessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
                }

                var response = await _httpClient.GetAsync("/api/requestoff", cancellationToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<List<RequestOffDto>>(jsonString, options);
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

        public async Task<bool> ApproveRequestAsync(long requestId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Get JWT token
                var loginResponse = await LoginAndGetTokenAsync();
                if (!string.IsNullOrEmpty(loginResponse?.AccessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
                }

                // Bodge fix sine
                var loginRequest = new
                {
                    email = "test",
                    password = "test"
                };

                var loginJson = JsonSerializer.Serialize(loginRequest);
                var loginContent = new StringContent(loginJson, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/requestoff/approve/{requestId}", loginContent, cancellationToken);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DenyRequestAsync(long requestId, CancellationToken cancellationToken = default)
        {
            try
            {
                // Get JWT token
                var loginResponse = await LoginAndGetTokenAsync();
                if (!string.IsNullOrEmpty(loginResponse?.AccessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);
                }

                // Bodge fix sine
                var loginRequest = new
                {
                    email = "test",
                    password = "test"
                };

                var loginJson = JsonSerializer.Serialize(loginRequest);
                var loginContent = new StringContent(loginJson, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/requestoff/deny/{requestId}", loginContent, cancellationToken);

                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
        }
    }

