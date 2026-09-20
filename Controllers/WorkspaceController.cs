using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs.Workspace;
using StudyHubAPI.Services;

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
                return NotFound("No Workspace is found");
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
                return BadRequest("WorkspaceID can't be zero or less");
            }

            var workspace = await _workspaceService.GetWorkspaceByID(workspaceID);

            if (workspace == null)
            {
                return NotFound("No workspace was found");
            }

            return Ok(workspace);

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
                return BadRequest("WorkspaceID can not zero or less");
            }


            var succeeded = await _workspaceService.UpdateWorkspace(workspaceID,dto);

            if (!succeeded)
            {
                return NotFound("Was not found");
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
                return BadRequest("WorkspaceID can not zero or less");
            }


            var succeeded = await _workspaceService.SetWorkSpaceToMaintence(workspaceID);

            if (!succeeded)
            {
                return NotFound("Was not found");
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


            var succeeded = await _workspaceService.SetWorkspaceToAvailable(workspaceID);

            if (!succeeded)
            {
                return NotFound("Was not found");
            }

            return NoContent();
        }



    }
}
