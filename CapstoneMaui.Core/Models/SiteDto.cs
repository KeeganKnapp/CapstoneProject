namespace CapstoneMaui.Core.Models;

public record class SiteDto
{
    public Guid Id { get; init; }
    public SiteDto(Guid Id, string Name, double Lat, double Lng, int RadiusMeters)
    {
        this.Id = Id;
        this.Name = Name;
        this.Lat = Lat;
        this.Lng = Lng;
        this.RadiusMeters = RadiusMeters;
    }
    public double Lat { get; init; }
    public double Lng { get; init; }
    public int RadiusMeters { get; init; }
    public string Name { get; init; }

}
