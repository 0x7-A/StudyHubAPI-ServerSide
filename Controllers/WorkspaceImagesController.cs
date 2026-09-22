using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using StudyHubAPI.Services;
using StudyHubAPI.Models.DTOs.WorkspaceImages;
using Microsoft.AspNetCore.Mvc;

namespace StudyHubAPI.Controllers
{
    [Route("api/WorkspaceImages")]
    [ApiController]
    public class WorkspaceImagesController : ControllerBase
    {
        private readonly WorkspaceImagesService _workspaceImagesService;

        public WorkspaceImagesController(WorkspaceImagesService workspaceImagesService)
        {
            _workspaceImagesService = workspaceImagesService;
        }   

        [Authorize(Roles = "Admin")]
        [HttpPost("Add/{workspaceId}", Name = "UploadWorkspaceImage")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<ActionResult> UploadWorkspaceImage(int workspaceId, UploadWorkspaceImageDto dto)
        {
            if (workspaceId <= 0)
            {
                return BadRequest("workspaceId can't be zero or less");
            }

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("Please select an image file.");

          
            var result = await _workspaceImagesService.UploadWorkspaceImage(workspaceId, dto.File);

            return Ok(new { message = "Workspace image uploaded successfully." });
        }
    }
}
