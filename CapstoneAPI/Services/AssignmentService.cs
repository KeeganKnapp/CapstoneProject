using CapstoneAPI.Data;
using CapstoneAPI.Dtos;
using CapstoneAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneAPI.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly CapstoneDbContext _db;

        public AssignmentService(CapstoneDbContext db)
        {
            _db = db;
        }

        //
        // assignment CRUD
        //

        public async Task<AssignmentResponse> CreateAssignmentAsync(int creatorUserId, AssignmentCreateRequest request)
        {
            await EnsureManagerAsync(creatorUserId);

            var jobsiteExists = await _db.Jobsites
                .AnyAsync(j => j.JobsiteId == request.JobsiteId);

            if (!jobsiteExists)
                throw new KeyNotFoundException("Jobsite not found.");

            var assignment = new Assignment
            {
                JobsiteId = request.JobsiteId,
                Title = request.Title.Trim(),
                Descriptiion = request.Description,
                Status = "todo",
                TotalHours = 0,
                CreatedByUserId = creatorUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Assignments.Add(assignment);
            await _db.SaveChangesAsync(); // gets AssignmentId

            if (request.AssignedUserIds != null && request.AssignedUserIds.Count > 0)
            {
                var distinctUserIds = request.AssignedUserIds.Distinct().ToList();

                foreach (var userId in distinctUserIds)
                {
                    _db.UserAssignments.Add(new UserAssignment
                    {
                        AssignmentId = assignment.AssignmentId,
                        UserId = userId
                    });
                }

                await _db.SaveChangesAsync();
            }

            var loaded = await _db.Assignments
                .Include(a => a.UserAssignments)
                .FirstAsync(a => a.AssignmentId == assignment.AssignmentId);

            return ToDto(loaded);
        }

        public async Task<AssignmentResponse> GetByIdAsync(int assignmentId, int requesterUserId)
        {
            var assignment = await _db.Assignments
                .Include(a => a.UserAssignments)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
                throw new KeyNotFoundException("Assignment not found.");

            var isManager = await IsManagerAsync(requesterUserId);
            var isAssigned = assignment.UserAssignments.Any(ua => ua.UserId == requesterUserId);

            if (!isManager && !isAssigned)
                throw new UnauthorizedAccessException("You do not have access to this assignment.");

            return ToDto(assignment);
        }

        public async Task<IEnumerable<AssignmentResponse>> GetForJobsiteAsync(int jobsiteId, int requesterUserId)
        {
            await EnsureManagerAsync(requesterUserId);

            var assignments = await _db.Assignments
                .Where(a => a.JobsiteId == jobsiteId)
                .Include(a => a.UserAssignments)
                .ToListAsync();

            return assignments.Select(ToDto);
        }

        public async Task<IEnumerable<AssignmentResponse>> GetForUserAsync(int userId, string? status)
        {
            var query = _db.Assignments
                .Include(a => a.UserAssignments)
                .Where(a => a.UserAssignments.Any(ua => ua.UserId == userId));

            if (!string.IsNullOrWhiteSpace(status))
            {
                var normalized = NormalizeStatus(status);
                query = query.Where(a => a.Status == normalized);
            }

            var assignments = await query.ToListAsync();
            return assignments.Select(ToDto);
        }

        public async Task<AssignmentResponse> UpdateAsync(int assignmentId, int requesterUserId, AssignmentUpdateRequest request)
        {
            await EnsureManagerAsync(requesterUserId);

            var assignment = await _db.Assignments
                .Include(a => a.UserAssignments)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
                throw new KeyNotFoundException("Assignment not found.");

            if (request.Title != null)
                assignment.Title = request.Title.Trim();

            if (request.Description != null)
                assignment.Descriptiion = request.Description;

            if (request.AssignedUserIds != null)
            {
                var newIds = request.AssignedUserIds.Distinct().ToList();

                var toRemove = assignment.UserAssignments
                    .Where(ua => !newIds.Contains(ua.UserId))
                    .ToList();

                _db.UserAssignments.RemoveRange(toRemove);

                var existingIds = assignment.UserAssignments
                    .Select(ua => ua.UserId)
                    .ToHashSet();

                var toAdd = newIds.Where(id => !existingIds.Contains(id)).ToList();

                foreach (var userId in toAdd)
                {
                    _db.UserAssignments.Add(new UserAssignment
                    {
                        AssignmentId = assignment.AssignmentId,
                        UserId = userId
                    });
                }
            }

            assignment.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var reloaded = await _db.Assignments
                .Include(a => a.UserAssignments)
                .FirstAsync(a => a.AssignmentId == assignment.AssignmentId);

            return ToDto(reloaded);
        }

        public async Task<AssignmentResponse> UpdateStatusAsync(int assignmentId, int requesterUserId, string status)
        {
            var normalizedStatus = NormalizeStatus(status);

            var assignment = await _db.Assignments
                .Include(a => a.UserAssignments)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
                throw new KeyNotFoundException("Assignment not found.");

            var isManager = await IsManagerAsync(requesterUserId);
            var isAssigned = assignment.UserAssignments.Any(ua => ua.UserId == requesterUserId);

            if (!isManager && !isAssigned)
                throw new UnauthorizedAccessException("You are not allowed to update this assignment.");

            assignment.Status = normalizedStatus;
            assignment.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return ToDto(assignment);
        }

        public async Task DeleteAsync(int assignmentId, int requesterUserId)
        {
            await EnsureManagerAsync(requesterUserId);

            var assignment = await _db.Assignments
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
                throw new KeyNotFoundException("Assignment not found.");

            _db.Assignments.Remove(assignment);
            await _db.SaveChangesAsync();
        }

        // 
        // user assignment actions
        // 

        public async Task AddUserToAssignmentAsync(int assignmentId, int targetUserId, int requesterUserId)
        {
            await EnsureManagerAsync(requesterUserId);

            var exists = await _db.Assignments
                .AnyAsync(a => a.AssignmentId == assignmentId);

            if (!exists)
                throw new KeyNotFoundException("Assignment not found.");

            var alreadyAssigned = await _db.UserAssignments
                .AnyAsync(ua => ua.AssignmentId == assignmentId && ua.UserId == targetUserId);

            if (alreadyAssigned)
                return;

            _db.UserAssignments.Add(new UserAssignment
            {
                AssignmentId = assignmentId,
                UserId = targetUserId
            });

            await _db.SaveChangesAsync();
        }

        public async Task RemoveUserFromAssignmentAsync(int assignmentId, int targetUserId, int requesterUserId)
        {
            await EnsureManagerAsync(requesterUserId);

            var ua = await _db.UserAssignments
                .FirstOrDefaultAsync(x => x.AssignmentId == assignmentId && x.UserId == targetUserId);

            if (ua == null)
                return;

            _db.UserAssignments.Remove(ua);
            await _db.SaveChangesAsync();
        }

        //
        // assignment comments
        // 

        public async Task<AssignmentCommentResponse> AddCommentAsync(int assignmentId, int userId, AssignmentCommentCreateRequest request)
        {
            var assignment = await _db.Assignments
                .Include(a => a.UserAssignments)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
                throw new KeyNotFoundException("Assignment not found.");

            var isManager = await IsManagerAsync(userId);
            var isAssigned = assignment.UserAssignments.Any(ua => ua.UserId == userId);

            if (!isManager && !isAssigned)
                throw new UnauthorizedAccessException("You are not allowed to comment on this assignment.");

            var comment = new AssignmentComment
            {
                AssignmentId = assignmentId,
                UserId = userId,
                Text = request.Text,
                CreatedAt = DateTime.UtcNow
            };

            _db.AssignmentComments.Add(comment);
            await _db.SaveChangesAsync();

            return ToCommentDto(comment);
        }

        public async Task<IEnumerable<AssignmentCommentResponse>> GetCommentsAsync(int assignmentId, int requesterUserId)
        {
            var assignment = await _db.Assignments
                .Include(a => a.UserAssignments)
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);

            if (assignment == null)
                throw new KeyNotFoundException("Assignment not found.");

            var isManager = await IsManagerAsync(requesterUserId);
            var isAssigned = assignment.UserAssignments.Any(ua => ua.UserId == requesterUserId);

            if (!isManager && !isAssigned)
                throw new UnauthorizedAccessException("You are not allowed to view comments for this assignment.");

            var comments = await _db.AssignmentComments
                .Where(c => c.AssignmentId == assignmentId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return comments.Select(ToCommentDto);
        }

        //
        // helpers
        //

        private async Task<bool> IsManagerAsync(int userId)
        {
            var role = await _db.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.Role)
                .FirstOrDefaultAsync();

            return string.Equals(role, "Manager", StringComparison.OrdinalIgnoreCase);
        }

        private async Task EnsureManagerAsync(int userId)
        {
            if (!await IsManagerAsync(userId))
                throw new UnauthorizedAccessException("Only managers can perform this action.");
        }

        private static string NormalizeStatus(string status)
        {
            var normalized = status.Trim().ToLowerInvariant();

            if (normalized != "todo" &&
                normalized != "in_progress" &&
                normalized != "done")
            {
                throw new ArgumentException("Invalid status. Must be 'todo', 'in_progress', or 'done'.");
            }

            return normalized;
        }

        private AssignmentResponse ToDto(Assignment assignment)
        {
            return new AssignmentResponse
            {
                AssignmentId = assignment.AssignmentId,
                JobsiteId = assignment.JobsiteId,
                Title = assignment.Title,
                Description = assignment.Descriptiion, // model typo mapped back to DTO
                Status = assignment.Status,
                TotalHours = assignment.TotalHours,
                CreatedByUserId = assignment.CreatedByUserId,
                CreatedAt = assignment.CreatedAt,
                UpdatedAt = assignment.UpdatedAt,
                AssignedUserIds = assignment.UserAssignments
                    .Select(ua => ua.UserId)
                    .ToList()
            };
        }

        private AssignmentCommentResponse ToCommentDto(AssignmentComment comment)
        {
            return new AssignmentCommentResponse
            {
                CommentId = comment.CommentId,
                AssignmentId = comment.AssignmentId,
                UserId = comment.UserId,
                Text = comment.Text,
                CreatedAt = comment.CreatedAt
            };
        }
    }
}