using CapstoneAPI.Data;
using CapstoneAPI.Dtos;
using CapstoneAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneAPI.Services
{
    public class JobsiteService : IJobsiteService
    {
        private readonly CapstoneDbContext _db;

        public JobsiteService(CapstoneDbContext db)
        {
            _db = db;
        }

        public async Task<JobsiteResponse> CreateJobsiteAsync(CreateJobsiteRequest req, CancellationToken ct) // creating new jobsite
        {
            var now = DateTime.UtcNow;

            var jobsite = new Jobsite
            {
                Name = req.Name,
                Latitude = req.Latitude,
                Longitude = req.Longitude,
                RadiusMeters = req.RadiusMeters,
                CreatedAt = now,
                UpdatedAt = now
            };

            _db.Jobsites.Add(jobsite);
            await _db.SaveChangesAsync(ct);

            return Map(jobsite);
        }

        public async Task<JobsiteResponse> UpdateJobsiteAsync(int jobsiteId, UpdateJobsiteRequest req, CancellationToken ct) // updating existing jobsite
        {
            var jobsite = await _db.Jobsites.FirstOrDefaultAsync(j => j.JobsiteId == jobsiteId, ct);

            if (jobsite == null)
                throw new KeyNotFoundException($"Jobsite {jobsiteId} not found.");

            if (req.Name != null)
                jobsite.Name = req.Name;

            if (req.Latitude.HasValue)
                jobsite.Latitude = req.Latitude.Value;

            if (req.Longitude.HasValue)
                jobsite.Longitude = req.Longitude.Value;

            if (req.RadiusMeters.HasValue)
                jobsite.RadiusMeters = req.RadiusMeters.Value;

            jobsite.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);

            return Map(jobsite);
        }


        public async Task DeleteJobsiteAsync(int jobsiteId, CancellationToken ct) // deleting jobsite
        {
            var jobsite = await _db.Jobsites.FirstOrDefaultAsync(j => j.JobsiteId == jobsiteId, ct);

            if (jobsite == null)
                throw new KeyNotFoundException($"Jobsite {jobsiteId} not found.");

            _db.Jobsites.Remove(jobsite);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<JobsiteResponse> GetJobsiteByIdAsync(int jobsiteId, CancellationToken ct) // getting jobsite by id
        {
            var jobsite = await _db.Jobsites.AsNoTracking().FirstOrDefaultAsync(j => j.JobsiteId == jobsiteId, ct);

            if (jobsite == null)
                throw new KeyNotFoundException($"Jobsite {jobsiteId} not found.");

            return Map(jobsite);
        }

        public async Task<List<JobsiteResponse>> GetAllJobsiteAsync(CancellationToken ct) // getting all jobsites
        {
            var jobsites = await _db.Jobsites.AsNoTracking().ToListAsync(ct);
            return jobsites.Select(Map).ToList();
        }

        // Convert entity → DTO
        private JobsiteResponse Map(Jobsite j)
        {
            return new JobsiteResponse
            {
                JobsiteId = j.JobsiteId,
                Name = j.Name,
                Latitude = j.Latitude,
                Longitude = j.Longitude,
                RadiusMeters = j.RadiusMeters,
                CreatedAt = j.CreatedAt,
                UpdatedAt = j.UpdatedAt
            };
        }
    }
}