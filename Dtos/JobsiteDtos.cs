namespace CapstoneBlazorApp.Dtos
{
    // used when creating a brand new jobsite
    public class CreateJobsiteRequest
    {
        public string Name { get; set; } = null!; // jobsite name
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double RadiusMeters { get; set; } // radius in meters defines how close user must be to clock in
    }


    // used for updates to an existing jobsite
    // all fields are nullable
    public class UpdateJobsiteRequest
    {
        public string? Name { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? RadiusMeters { get; set; }
    }



    // returned to the frontend when showing jobsite data
    public class JobsiteResponse
    {

        public int JobsiteId { get; set; }
        public string Name { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double RadiusMeters { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}