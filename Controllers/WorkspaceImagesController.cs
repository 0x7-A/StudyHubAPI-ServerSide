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
                return BadRequest(new { error = "workspaceId can't be zero or less" });
            }

     
            var result = await _workspaceImagesService.UploadWorkspaceImage(workspaceId, dto.File);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, new { error = result.ErrorMessage });
            }

            return StatusCode(result.StatusCode, result.Data);
        }


        [HttpGet("workspace/{workspaceId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<WorkspaceImageResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetImagesByWorkspace(int workspaceId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var images = await _workspaceImagesService.GetWorkspaceImagesAsync(workspaceId, baseUrl);

            if (images is null)
                return NotFound(new { error = $"Workspace with ID {workspaceId} does not exist." });

            return Ok(images);
        }

        [AllowAnonymous]
        [HttpGet("{imageId:int}/file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetImageFile(int imageId)
        {
            var fileDto = await _workspaceImagesService.GetImageFileAsync(imageId);

            if (!fileDto.IsSuccess)
                return StatusCode(fileDto.StatusCode, new { error = fileDto.ErrorMessage });

 
            return PhysicalFile(fileDto.Data.FilePath, fileDto.Data.ContentType);
        }


        [HttpDelete("{Id:int}", Name = "DeleteWorkspaceImage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteWorkspaceImage(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest(new { error = "Image ID can't be zero or less" }  );
            }

            var result = await _workspaceImagesService.DeleteWorkspaceImage(Id);

            if (result.IsSuccess)
            {
                return StatusCode(result.StatusCode);
            }

            return StatusCode(result.StatusCode, new { error = result.ErrorMessage });
        }


    }
}
