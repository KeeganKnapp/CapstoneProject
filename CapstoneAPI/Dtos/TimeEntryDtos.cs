namespace CapstoneAPI.Dtos;

public record ClockRequest(int jobsiteiD, double latitute, double longitude);
public record TimeEntryDto(int id, DateTimeOffset TimestampUtc, int jobsiteiD, string type); // in / out
