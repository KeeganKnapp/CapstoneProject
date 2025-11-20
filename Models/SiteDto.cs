    namespace CapstoneBlazorApp.Models {
    public class SiteDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public double RadiusMeters { get; set; }

        public SiteDto(Guid id, string name, double lat, double lng, double radiusMeters)
        {
            Id = id;
            Name = name;
            Lat = lat;
            Lng = lng;
            RadiusMeters = radiusMeters;
        }
    }
    }
