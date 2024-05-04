using Minio.DataModel.Args;
using Minio;
using Services.Contracts;
using Entities.Exceptions.File;
using Microsoft.Extensions.Options;
using Entities.UtilityClasses.Minio;

namespace Services
{
    public class FileDownloadManager(
        ILoggerService logger,
        IOptions<CustomMinioConfig> minioConfig,
        IMinioClient minioClient) : IFileDownloadService
    {
        private readonly IMinioClient _minioClient = minioClient;
        private readonly ILoggerService _logger = logger;


        private readonly CustomMinioConfig minioConfig = minioConfig.Value;

        public async Task<(MemoryStream fileStream, string contentType)> Download(string fileName, string filePath)
        {

            MemoryStream memoryStream = new()
            {
                Position = 0
            };

            try
            {
                var objectStatArgs = new StatObjectArgs()
                     .WithBucket(minioConfig.Bucket)
                     .WithObject(String.Concat(filePath, fileName));
                var statObject = await _minioClient.StatObjectAsync(objectStatArgs).ConfigureAwait(false);


                var args = new GetObjectArgs()
                     .WithBucket(minioConfig.Bucket)
                     .WithObject(String.Concat(filePath, fileName))
                     .WithCallbackStream((stream) =>
                     {
                         stream.CopyTo(memoryStream);
                     });

                var s = await _minioClient.GetObjectAsync(args).ConfigureAwait(false);

                return (memoryStream, s.ContentType);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Method: {nameof(Download)}; An error occurred while download the media, {ex}");
                throw new FileNameNotFoundException();
            }
        }
    }
}
