using Asp.Versioning;
using Entities.Enums;
using Entities.UtilityClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;


namespace Presentation.Controllers.v1
{

    [ApiVersion(1)]
    [ApiController]
    [Route("api/v{v:apiVersion}/files")]
    public class FileController(IFileDownloadService fileDownloadService, IServiceManager serviceManager) : ControllerBase
    {

        private readonly IFileDownloadService _fileDownloadService = fileDownloadService;
        private readonly IServiceManager _serviceManager = serviceManager;

        [Authorize(Roles = Roles.User)]
        [HttpGet("{folderPath}/{fileName}")]
        public async Task<IActionResult> Download([FromRoute] string folderPath, [FromRoute] string fileName)
        {

            if (!Enum.TryParse(folderPath, out FolderPaths path))
            {
                ModelState.AddModelError("Message", "Invalid media path.");
                return BadRequest(ModelState);
            }

            if (path.Equals(FolderPaths.Tweets))
            {
                string filePath = await _serviceManager.TweetsService.CheckTweetMediaViewPermission(fileName, User.Identity.Name);
                fileName = filePath.Split('/')[^1]; // ex: /Tweets/765945d3-7662-47eb-afda-a6c84f4e54ad.png
            }

            var result = await _fileDownloadService.Download(fileName, path.GetDescription());
            return File(result.fileStream.ToArray(), result.contentType, fileName);
        }



    }
}
