using System;
using CapstoneBlazorApp.Services.Abstractions;

namespace CapstoneBlazorApp.Services;

public class LocationManager
{

    private readonly AbstractLoggerService _logger;

    public LocationManager(AbstractLoggerService logger)
    {
        _logger = logger;
    }

    public double GetDistance(double lat1, double lon1, double lat2, double lon2) {
        var R = 6371e3; // metres
        var φ1 = lat1 * Math.PI / 180; // φ, λ in radians
        var φ2 = lat2 * Math.PI / 180;
        var Δφ = (lat2 - lat1) * Math.PI / 180;
        var Δλ = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                Math.Cos(φ1) * Math.Cos(φ2) *
                Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        var d = R * c; // in metres
        return d;
    }

    public async Task<bool> IsWithinRadiusAsync(double siteLat, double siteLng, double radiusMeters, CancellationToken cancellationToken = default) {
        var (currentLat, currentLng) = await GetCurrentLocationAsync(cancellationToken);
        double distance = GetDistance(currentLat, currentLng, siteLat, siteLng);
        _logger.Log(this,$"Current Location: ({currentLat}, {currentLng}), Site Location: ({siteLat}, {siteLng}), Distance: {distance} meters, Site Radius: {radiusMeters} meters", "info");
        return distance <= radiusMeters;
    }    public async Task<(double Latitude, double Longitude)> GetCurrentLocationAsync(CancellationToken cancellationToken = default)
    {
        _logger.Log(this, "Attempting to retrieve current location", "info");
        
        // For demo purposes, we'll simulate being at the first jobsite location
        // In a real implementation, you would use JavaScript interop to call navigator.geolocation
        await Task.Delay(1000, cancellationToken); // Simulate API delay
        
        // Simulate being at Downtown Construction Site location for demo
        var demoLatitude = 41.1450;
        var demoLongitude = -81.3416;
        
        _logger.Log(this, $"Demo location retrieved: ({demoLatitude}, {demoLongitude})", "info");
        return (demoLatitude, demoLongitude);
        
        // TODO: Implement real browser geolocation using JavaScript interop
        // Example implementation would look like:
        /*
        try
        {
            // Call JavaScript function that uses navigator.geolocation.getCurrentPosition
            var position = await _jsRuntime.InvokeAsync<GeolocationPosition>("getLocation", cancellationToken);
            if (position != null)
            {
                _logger.Log(this, $"Location retrieved: ({position.Latitude}, {position.Longitude})", "info");
                return (position.Latitude, position.Longitude);
            }
            else
            {
                _logger.Log(this, "Unable to retrieve current location", "error");
                throw new Exception("Unable to retrieve current location");
            }
        }
        catch (Exception ex)
        {
            _logger.Log(this, $"Location retrieval failed: {ex.Message}", "error");
            throw;
        }
        */
    }
}

