

using CapstoneAPI.Dtos;

namespace CapstoneAPI.Services
{
    public interface IAssignmentService
    {
        // assignment CRUD
        Task<AssignmentResponse> CreateAssignmentAsync(int creatorUserId, AssignmentCreateRequest request);
        Task<AssignmentResponse> GetByIdAsync(int assignmentId, int requesterUserId);
        Task<IEnumerable<AssignmentResponse>> GetForJobsiteAsync(int jobsiteId, int requesterUserId);
        Task<IEnumerable<AssignmentResponse>> GetForUserAsync(int userId, string? status);
        Task<AssignmentResponse> UpdateAsync(int assignmentId, int requesterUserId, AssignmentUpdateRequest request);
        Task<AssignmentResponse> UpdateStatusAsync(int assignmentId, int requesterUserId, string status);
        Task DeleteAsync(int assignmentId, int requesterUserId);

        // user assignments
        Task AddUserToAssignmentAsync(int assignmentId, int targetUserId, int requesterUserId);
        Task RemoveUserFromAssignmentAsync(int assignmentId, int targetUserId, int requesterUserId);

        // comments
        Task<AssignmentCommentResponse> AddCommentAsync(int assignmentId, int userId, AssignmentCommentCreateRequest request);
        Task<IEnumerable<AssignmentCommentResponse>> GetCommentsAsync(int assignmentId, int requesterUserId);
    }
}