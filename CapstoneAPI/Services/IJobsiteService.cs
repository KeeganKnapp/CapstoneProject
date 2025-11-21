using CapstoneAPI.Dtos;

namespace CapstoneAPI.Services
{
    // defines the operations the Jobsite service must provide
    public interface IJobsiteService
    {
        Task<JobsiteResponse> CreateJobsiteAsync(CreateJobsiteRequest req, CancellationToken ct); // new jobsite
        Task<JobsiteResponse> UpdateJobsiteAsync(int jobsiteId, UpdateJobsiteRequest req, CancellationToken ct); // update existing jobsite
        Task DeleteJobsiteAsync(int jobsiteId, CancellationToken ct); // delete a jobsite
        Task<JobsiteResponse> GetJobsiteByIdAsync(int jobsiteId, CancellationToken ct); // fetching single jobsite by id
        Task<List<JobsiteResponse>> GetAllJobsiteAsync(CancellationToken ct); // list of all jobsites
    }
}