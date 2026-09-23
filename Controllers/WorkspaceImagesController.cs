using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs.WorkspaceImages;
using StudyHubAPI.Services;
using StudyHubAPI.Utils;

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
                return BadRequest(new { Error = "workspaceId can't be zero or less" });
            }

     
            var result = await _workspaceImagesService.UploadWorkspaceImage(workspaceId, dto.File);
            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.BadRequest => BadRequest(new { error = result.ErrorMessage }),
                    ResultType.Failure => BadRequest(new { error = result.ErrorMessage ?? "Operation failed." })
                };
            }

            return NoContent();
        }


        [HttpGet("workspace/{workspaceId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<WorkspaceImageResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetImagesByWorkspace(int workspaceId)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var images = await _workspaceImagesService.GetWorkspaceImagesAsync(workspaceId, baseUrl);

            if (images is null)
                return NotFound(new { Error = $"Workspace with ID {workspaceId} does not exist." });

            return Ok(images);
        }

        [AllowAnonymous]
        [HttpGet("{imageId:int}/file")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult> GetImageFile(int imageId)
        {
            if (imageId <= 0)
            {
                return BadRequest(new { error = "Image ID can't be zero or less" });
            }

            var result = await _workspaceImagesService.GetImageFileAsync(imageId);

            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.BadRequest => BadRequest(new { error = result.ErrorMessage }),
                    ResultType.NotFound => NotFound(new { error = result.ErrorMessage }),
                    ResultType.Failure => BadRequest(new { error = result.ErrorMessage ?? "Operation failed." })
                };
            }


            return PhysicalFile(result.Data.FilePath, result.Data.ContentType);
        }


        [HttpDelete("{Id:int}", Name = "DeleteWorkspaceImage")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteWorkspaceImage(int Id)
        {
            if (Id <= 0)
            {
                return BadRequest(new { error = "Image ID can't be zero or less" });
            }

            var result = await _workspaceImagesService.DeleteWorkspaceImage(Id);

            if (!result.IsSuccess)
            {
                return result.Type switch
                {
                    ResultType.BadRequest => BadRequest(new { error = result.ErrorMessage }),
                    ResultType.NotFound => NotFound(new { error = result.ErrorMessage }),
                    ResultType.Failure => BadRequest(new { error = result.ErrorMessage ?? "Operation failed." })
                };
            }


            return NoContent();
        }


    }
}
