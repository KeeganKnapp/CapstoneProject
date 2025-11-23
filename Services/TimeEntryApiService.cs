using CapstoneBlazorApp.Dtos;
using CapstoneBlazorApp.Services.Abstractions;
using System.Net.Http.Json;
using System.Text.Json;

namespace CapstoneBlazorApp.Services
{
    public class TimeEntryApiService
    {
        private readonly HttpClient _httpClient;
        private readonly AbstractLoggerService _logger;

        public TimeEntryApiService(HttpClient httpClient, AbstractLoggerService logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<bool> ClockInAsync(int? assignmentId = null, DateTimeOffset? startTime = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new ClockInRequest
                {
                    AssignmentId = assignmentId,
                    StartTime = startTime
                };

                _logger.Log(this, $"Sending clock-in request: AssignmentId={assignmentId}", "info");

                var response = await _httpClient.PostAsJsonAsync("time-entries/clock-in", request, cancellationToken);
                
                _logger.Log(this, $"Clock-in response status: {response.StatusCode}", "info");
                
                if (response.IsSuccessStatusCode)
                {
                    var timeEntry = await response.Content.ReadFromJsonAsync<TimeEntryDto>(cancellationToken);
                    _logger.Log(this, $"Clock-in successful: TimeEntryId={timeEntry?.TimeEntryId}", "info");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.Log(this, $"Clock-in failed: {response.StatusCode} - {errorContent}", "error");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(this, $"Clock-in error: {ex.Message}", "error");
                return false;
            }
        }

        public async Task<bool> ClockOutAsync(long? timeEntryId = null, DateTimeOffset? endTime = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new ClockOutRequest
                {
                    TimeEntryId = timeEntryId,
                    EndTime = endTime
                };

                _logger.Log(this, $"Sending clock-out request: TimeEntryId={timeEntryId}", "info");

                var response = await _httpClient.PostAsJsonAsync("time-entries/clock-out", request, cancellationToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var timeEntry = await response.Content.ReadFromJsonAsync<TimeEntryDto>(cancellationToken);
                    _logger.Log(this, $"Clock-out successful: TimeEntryId={timeEntry?.TimeEntryId}", "info");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.Log(this, $"Clock-out failed: {response.StatusCode} - {errorContent}", "error");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Log(this, $"Clock-out error: {ex.Message}", "error");
                return false;
            }
        }

        public async Task<List<TimeEntryDto>> GetMyTimeEntriesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.Log(this, "Fetching user's time entries", "info");

                var response = await _httpClient.GetAsync("time-entries/mine", cancellationToken);
                
                if (response.IsSuccessStatusCode)
                {
                    var timeEntries = await response.Content.ReadFromJsonAsync<List<TimeEntryDto>>(cancellationToken) ?? new List<TimeEntryDto>();
                    _logger.Log(this, $"Retrieved {timeEntries.Count} time entries", "info");
                    return timeEntries;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.Log(this, $"Failed to fetch time entries: {response.StatusCode} - {errorContent}", "error");
                    return new List<TimeEntryDto>();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(this, $"Error fetching time entries: {ex.Message}", "error");
                return new List<TimeEntryDto>();
            }
        }

        public async Task<TimeEntryDto?> GetCurrentOpenTimeEntryAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var timeEntries = await GetMyTimeEntriesAsync(cancellationToken);
                var openEntry = timeEntries.FirstOrDefault(te => te.EndTime == null);
                
                if (openEntry != null)
                {
                    _logger.Log(this, $"Found open time entry: TimeEntryId={openEntry.TimeEntryId}", "info");
                }
                else
                {
                    _logger.Log(this, "No open time entries found", "info");
                }

                return openEntry;
            }
            catch (Exception ex)
            {
                _logger.Log(this, $"Error checking for open time entry: {ex.Message}", "error");
                return null;
            }
        }
    }
}
