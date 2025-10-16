namespace CapstoneAPI.Dtos;

public record LoginRequest(string username, string password);
public record LoginResponse(string token, string role);