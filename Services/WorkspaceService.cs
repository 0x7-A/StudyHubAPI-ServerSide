using StudyHubAPI.Repositories;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Models.DTOs.Workspace;


namespace StudyHubAPI.Services
{
    public class WorkspaceService
    {

        private readonly  WorkspaceRepository _WorkspaceRepository;

        public WorkspaceService(WorkspaceRepository workspaceRepository)
        {
            _WorkspaceRepository = workspaceRepository;
        }


        public async Task<WorkspaceSummaryDto?> GetWorkspaceByID(int WorkspaceID)
        {
           var workspace = await _WorkspaceRepository.GetWorkSpaceByID(WorkspaceID);

            if (workspace == null)
            {
                return null;
            }

            return new WorkspaceSummaryDto { WorkspaceID = workspace.WorkspaceID ,Description = workspace.Description, HourlyRate = workspace.HourlyRate,
                MaximumCapacity = workspace.MaximumCapacity, WorkspaceStatus = workspace.WorkspaceStatus } ;
          
        }

        public Task<List<WorkspaceSummaryDto>> GetAllWorkspace()
        {
            return _WorkspaceRepository.GetAllWorkspaces();
        }

        public async Task<int> AddNewWorkSpace(CreateWorkspaceDto NewWorkSpace)
        {
            var Workspace = new Workspaces { Description = NewWorkSpace.Description, HourlyRate = NewWorkSpace.HourlyRate,
                WorkspaceStatus = NewWorkSpace.WorkspaceStatus, MaximumCapacity = NewWorkSpace.MaximumCapacity };

            return await _WorkspaceRepository.AddWorkspace(Workspace);

        }


        public async Task<bool> UpdateWorkspace(int workspaceId, UpdateWorkspaceDto dto)
        {
            Workspaces? workspace = await _WorkspaceRepository.GetWorkSpaceByID(workspaceId);

            if (workspace == null)
            {
                return false;
            }

            if (dto.Description != null)
            {
                workspace.Description = dto.Description;
            }

            if (dto.HourlyRate != null)
            {
                workspace.HourlyRate = dto.HourlyRate.Value;
            }

            if (dto.MaximumCapacity != null)
            {
                workspace.MaximumCapacity = dto.MaximumCapacity.Value;
            }


            return await _WorkspaceRepository.SaveChangesAsync() > 0;

        }


        public async  Task<bool> SetWorkSpaceToMaintence(int workSpaceID)
        {

            // in next versions, I will do action for next reservation on the specfiied workspace

            return await _WorkspaceRepository.UpdateWorkspaceStatus(workSpaceID, WorkspaceStatus.UnderMaintenance) > 0;
        }


        public async Task<bool> SetWorkspaceToAvailable(int workSpaceID)
        {
            return await _WorkspaceRepository.UpdateWorkspaceStatus(workSpaceID, WorkspaceStatus.Available) > 0;
        }

        public async Task<bool> DeleteWorkspace()
        {
            // will be implmented in next extention 

            // to do an event for next reservetion
            // change the status of next reservtion 
            // mark for deletion

            return false;
        }



    }
}
