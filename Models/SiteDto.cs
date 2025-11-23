namespace CapstoneBlazorApp.Models
{
    public record SiteDto(int Id, string Name, double Lat, double Lng, double RadiusMeters);

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
