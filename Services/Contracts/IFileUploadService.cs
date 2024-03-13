using Entities.Enums;
using Microsoft.AspNetCore.Http;


namespace Services.Contracts
{
    public interface IFileUploadService
    {

        Task<string> Upload(IFormFile File, FolderPaths filePath, string? pastFileName);
        Task Delete(string filePath, string fileName);

    }
}
