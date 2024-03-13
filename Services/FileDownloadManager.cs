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
        IOptions<CustomMinioConfig> minioConfig) : IFileDownloadService
    {

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
                var minio = new MinioClient()
                                    .WithEndpoint(minioConfig.Endpoint)
                                    .WithCredentials(minioConfig.ReadOnlyAccess.AccessKey, minioConfig.ReadOnlyAccess.SecretKey)
                                    .WithSSL(minioConfig.SSL)
                                    .Build();


                var objectStatArgs = new StatObjectArgs()
                     .WithBucket(minioConfig.Bucket)
                     .WithObject(String.Concat(filePath, fileName));
                var statObject = await minio.StatObjectAsync(objectStatArgs).ConfigureAwait(false);


                var args = new GetObjectArgs()
                     .WithBucket(minioConfig.Bucket)
                     .WithObject(String.Concat(filePath, fileName))
                     .WithCallbackStream((stream) =>
                     {
                         stream.CopyTo(memoryStream);
                     });

                var s = await minio.GetObjectAsync(args).ConfigureAwait(false);

                return (memoryStream, s.ContentType);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Method: {nameof(Download)}; Dosya indirilirken bir hata oluştu, {ex}");
                throw new FileNameNotFoundException();
            }
        }
    }
}
