using Microsoft.EntityFrameworkCore;
using StudyHubAPI.Data;
using StudyHubAPI.Models.DTOs.Person;
using StudyHubAPI.Models.DTOs.Workspace;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Repositories;

namespace StudyHubAPI.Repositories
{
    public class WorkspaceRepository
    {
        private readonly StudyHubDbContext _context;
        public WorkspaceRepository(StudyHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkspaceSummaryDto>> GetAllWorkspaces()
        {
            return await _context.Workspaces
                .AsNoTracking()
                .Select(w => new WorkspaceSummaryDto
                {
                    WorkspaceID = w.WorkspaceID,
                    Description = w.Description,
                    WorkspaceStatus = w.WorkspaceStatus,
                    HourlyRate = w.HourlyRate,
                    MaximumCapacity = w.MaximumCapacity,
                }).ToListAsync();
        }

        public async Task<int> AddWorkspace(Workspaces workspace)
        {
            _context.Workspaces.Add(workspace);
            await _context.SaveChangesAsync();
            return workspace.WorkspaceID;
        }

        public async Task<int> DeleteWorkspace(int WorkspaceID)
        {
           return await _context.Workspaces.Where(w => w.WorkspaceID == WorkspaceID).ExecuteDeleteAsync();
        }

        public async Task<Workspaces?> GetWorkSpaceByID(int WorkSpaceID)
        {
            return await _context.Workspaces.FindAsync(WorkSpaceID);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetHourlyRate(int WorkplaceID)
        {
            return await _context.Workspaces.Where(w => w.WorkspaceID == WorkplaceID).Select(w => w.HourlyRate).SingleOrDefaultAsync();
        }

        public async Task<int> UpdateWorkspaceStatus(int workspaceID , WorkspaceStatus  workspaceStatus)
        {
            return await _context.Workspaces.Where(w => w.WorkspaceID == workspaceID).
                ExecuteUpdateAsync(setters => setters.SetProperty(w => w.WorkspaceStatus, w => (byte)workspaceStatus));
        }


    }
}
