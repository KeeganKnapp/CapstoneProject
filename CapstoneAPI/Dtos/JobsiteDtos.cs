namespace CapstoneAPI.Dtos
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
        // Unique ID of the jobsite
        public int JobsiteId { get; set; }

        // Name / label of this jobsite
        public string Name { get; set; } = null!;

        // Location coordinates
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Geofence radius in meters
        public double RadiusMeters { get; set; }

        // Time the jobsite was created
        public DateTime CreatedAt { get; set; }

        // Last update timestamp
        public DateTime UpdatedAt { get; set; }
    }
}