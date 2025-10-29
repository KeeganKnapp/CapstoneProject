using System;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using CapstoneMaui.Core.Services.Abstractions;

namespace CapstoneMaui.Core.Services.Utilities;

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
    }

    public Task<(double Latitude, double Longitude)> GetCurrentLocationAsync(CancellationToken cancellationToken = default) {
        _logger.Log(this, "Attempting to retrieve current location", "info");
        var status = Geolocation.Default.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)), cancellationToken);
        if (status.Result != null) {
            return Task.FromResult((status.Result.Latitude, status.Result.Longitude));
        }
        else {
            _logger.Log(this, "Unable to retrieve current location", "error");
            throw new Exception("Unable to retrieve current location");
        }
    }


}
