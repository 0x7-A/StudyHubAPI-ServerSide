using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs.Workspace;
using StudyHubAPI.Services;
using StudyHubAPI.Utils;

namespace StudyHubAPI.Controllers
{
    [Authorize]
    [Route("api/Workspace")]
    [ApiController]
    public class WorkspaceController : ControllerBase
    {
        WorkspaceService _workspaceService;

        public WorkspaceController(WorkspaceService workspaceService)
        {
            _workspaceService = workspaceService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Add", Name = "AddWorkspace")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<int>> AddWorkspace(CreateWorkspaceDto dto)
        {
            var newWorkspaceID = await _workspaceService.AddNewWorkSpace(dto);

            return CreatedAtRoute("GetWorkspaceByID", new { workspaceID = newWorkspaceID }, newWorkspaceID);
        }



        [HttpGet("All", Name = "GetAllWorkspaces")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<WorkspaceSummaryDto>>> GetAllWorkspaces()
        {
            var workspacesList = await _workspaceService.GetAllWorkspace();

            if (workspacesList.Count == 0)
            {
                return NotFound(new { Error = "No Workspace is found" } );
            }

            return Ok(workspacesList);
        }


        [HttpGet("{workspaceID:int}", Name = "GetWorkspaceByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<WorkspaceSummaryDto>> GetWorkspaceByID(int workspaceID)
        {
            if(workspaceID <= 0)
            {
                return BadRequest( new { Error = "WorkspaceID can't be zero or less" } );
            }

            var result = await _workspaceService.GetWorkspaceByID(workspaceID);

            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.NotFound => NotFound(new { Error = result.ErrorMessage }),
                    ResultType.Failure => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." }),
                      _ => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return Ok(result.Data);
        }



        [Authorize(Roles = "Admin")]
        [HttpPatch("UpdateWorkspace/{workspaceID:int}", Name = "UpdateWorkspace")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
       
        public async  Task<ActionResult> UpdateWorkspace(int workspaceID,  UpdateWorkspaceDto dto)
        {
            if(dto == null)
            {
                return BadRequest("Empty dto");
            }

            if (workspaceID <= 0)
            {
                return BadRequest(new { Error = "WorkspaceID can not zero or less" });
            }


            var result = await _workspaceService.UpdateWorkspace(workspaceID,dto);

            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.NotFound => NotFound(new { Error = result.ErrorMessage }),
                    ResultType.Failure => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." }),
                    _ => BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." })
                };
            }


            return NoContent();
           
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("SetWorkSpaceToMaintence/{workspaceID:int}", Name = "SetWorkspaceToMaintenanc")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult> SetWorkSpaceToMaintence(int workspaceID)
        {
            if (workspaceID <= 0)
            {
                return BadRequest(new { Error = "WorkspaceID can not zero or less" }    );
            }


            var result = await _workspaceService.SetWorkSpaceToMaintence(workspaceID);

            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." });
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("SetWorkspaceToAvailable/{workspaceID:int}", Name = "SetWorkspaceToAvailable")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SetWorkspaceToAvailable(int workspaceID)
        {

            if (workspaceID <= 0)
            {
                return BadRequest("WorkspaceID can not zero or less");
            }


            var result = await _workspaceService.SetWorkspaceToAvailable(workspaceID);

            if (!result.IsSuccess)
            {
                return BadRequest(new { Error = result.ErrorMessage ?? "Operation failed." });
            }

            return NoContent();
        }



    }
}
