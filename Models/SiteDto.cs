using System.Text.Json.Serialization;

namespace CapstoneBlazorApp.Models
{
    public record SiteDto(
        [property: JsonPropertyName("id")] int Id, 
        [property: JsonPropertyName("name")] string Name, 
        [property: JsonPropertyName("lat")] double Lat, 
        [property: JsonPropertyName("lng")] double Lng, 
        [property: JsonPropertyName("radiusMeters")] double RadiusMeters);

    // Extension method to convert from JobsiteResponse to SiteDto
    public static class SiteDtoExtensions
    {
        public static SiteDto ToSiteDto(this CapstoneBlazorApp.Dtos.JobsiteResponse jobsite)
        {
            return new SiteDto(
                jobsite.JobsiteId,
                jobsite.Name,
                jobsite.Latitude,
                jobsite.Longitude,
                jobsite.RadiusMeters
            );
        }
    }
}
