using Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;


namespace Presentation.Controllers.v1
{
    [ApiController]
    [Route("api/v1/files")]
    public class FileController(IFileUploadService fileUploadService, IFileDownloadService fileDownloadService) : ControllerBase
    {

        private readonly IFileUploadService _fileUploadService = fileUploadService; //test
        private readonly IFileDownloadService _fileDownloadService = fileDownloadService;

        [HttpPost] //test
        public async Task<IActionResult> Upload(IFormFile file)
        {
            await _fileUploadService.Upload(file, FolderPaths.TweetsImages, "3cdf3441-f1ea-43d1-8d13-280b752045f9.png");
            return StatusCode(201);
        }


        [HttpDelete("{fileName}")] //test
        public async Task<IActionResult> Delete([FromRoute] string fileName)
        {
            await _fileUploadService.Delete(FolderPaths.TweetsImages.GetDescription(), fileName);
            return NoContent();
        }



        [HttpGet("{folderPath}/{fileName}")]
        public async Task<IActionResult> Download([FromRoute] string folderPath, [FromRoute] string fileName)
        {

            if (!Enum.TryParse(folderPath, out FolderPaths path))
            {
                ModelState.AddModelError("Message", "Geçersiz dosya yolu.");
                return BadRequest(ModelState);
            }

            var result = await _fileDownloadService.Download(fileName, path.GetDescription());
            return File(result.fileStream.ToArray(), result.contentType, fileName);
        }

    }
}
