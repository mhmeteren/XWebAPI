namespace Services.Contracts
{
    public interface IFileDownloadService
    {
        Task<(MemoryStream fileStream, string contentType)> Download(string fileName, string filePath);
    }
}
