using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using StudyHubAPI.Services;
using StudyHubAPI.Models.DTOs.WorkspaceImages;
using Microsoft.AspNetCore.Mvc;

namespace StudyHubAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/WorkspaceImages")]
    [ApiController]
    public class WorkspaceImagesController : ControllerBase
    {
        private readonly WorkspaceImagesService _workspaceImagesService;

        public WorkspaceImagesController(WorkspaceImagesService workspaceImagesService)
        {
            _workspaceImagesService = workspaceImagesService;
        }   


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


        [HttpGet("workspace/{workspaceId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<WorkspaceImageResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetImagesByWorkspace(int workspaceId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var images = await _workspaceImagesService.GetWorkspaceImagesAsync(workspaceId, baseUrl);

            if (images is null)
                return NotFound($"Workspace with ID {workspaceId} does not exist.");

            return Ok(images);
        }

        [AllowAnonymous]
        [HttpGet("{imageId:int}/file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetImageFile(int imageId)
        {
            var fileDto = await _workspaceImagesService.GetImageFileAsync(imageId);

            if (fileDto is null)
                return NotFound("Image was not found or has been removed from disk.");

           
            return PhysicalFile(fileDto.FilePath, fileDto.ContentType);
        }


        [HttpDelete("{Id:int}", Name = "DeleteWorkspaceImage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteWorkspaceImage(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest("personID can't be zero or less");
            }

            var Succseeded = await _workspaceImagesService.DeleteWorkspaceImage(Id);

            if (Succseeded)
            {
                return NoContent();
            }

            return NotFound();
        }


    }
}
