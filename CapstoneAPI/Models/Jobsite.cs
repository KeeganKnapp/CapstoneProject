
/*


--------------------------------------------------------------------------------------
represents a work location that employees can be assigned to

*/

namespace CapstoneAPI.Models
{
    public class Jobsite
    {
        public int JobsiteId { get; set; } // primary key for table
        public string Name { get; set; } = null!; // name for jobsite
        public double Latitude { get; set; } 
        public double Longitude { get; set; }
        public double RadiusMeters { get; set; } // radius in meters
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // timestamp for when it was created
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // last updated at
    }
}