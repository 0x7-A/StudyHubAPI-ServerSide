using StudyHubAPI.Models.DTOs.Workspace;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Repositories;
using StudyHubAPI.Utils;


namespace StudyHubAPI.Services
{
    public class WorkspaceService
    {

        private readonly  WorkspaceRepository _WorkspaceRepository;

        public WorkspaceService(WorkspaceRepository workspaceRepository)
        {
            _WorkspaceRepository = workspaceRepository;
        }


        public async Task<ServiceResult<WorkspaceSummaryDto?>> GetWorkspaceByID(int WorkspaceID)
        {
           var workspace = await _WorkspaceRepository.GetWorkSpaceByID(WorkspaceID);

            if (workspace == null)
            {
                return ServiceResult<WorkspaceSummaryDto?>.Failure(ResultType.NotFound, "Workspace not found");
            }

            return ServiceResult<WorkspaceSummaryDto?>.Success( new WorkspaceSummaryDto
            {
                WorkspaceID = workspace.WorkspaceID,
                Description = workspace.Description,
                HourlyRate = workspace.HourlyRate,
                MaximumCapacity = workspace.MaximumCapacity,
                WorkspaceStatus = workspace.WorkspaceStatus
            },ResultType.Ok);
          
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


        public async Task<ServiceResult> UpdateWorkspace(int workspaceId, UpdateWorkspaceDto dto)
        {
            Workspaces? workspace = await _WorkspaceRepository.GetWorkSpaceByID(workspaceId);

            if (workspace == null)
            {
                return ServiceResult.Failure(ResultType.NotFound, "Workspace not found");
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


            if(await _WorkspaceRepository.SaveChangesAsync() > 0)
            {
                return ServiceResult.Success(ResultType.Ok);

            }
                return ServiceResult.Failure(ResultType.Failure, "Failed to update workspace");

        }


        public async  Task<ServiceResult> SetWorkSpaceToMaintence(int workSpaceID)
        {
            // in next versions, I will do action for next reservation on the specfiied workspace

            if (await _WorkspaceRepository.UpdateWorkspaceStatus(workSpaceID, WorkspaceStatus.UnderMaintenance) > 0)
            {
                return ServiceResult.Success(ResultType.Ok);
            }
            return ServiceResult.Failure(ResultType.Failure, "Failed to set workspace to maintenance");
        }


        public async Task<ServiceResult> SetWorkspaceToAvailable(int workSpaceID)
        {
            if (await _WorkspaceRepository.UpdateWorkspaceStatus(workSpaceID, WorkspaceStatus.Available) > 0)
            {
                return ServiceResult.Success(ResultType.Ok);
            }
            return ServiceResult.Failure(ResultType.Failure, "Failed to set workspace to maintenance");
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
