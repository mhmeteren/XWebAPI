using Entities.Enums;
using Entities.Exceptions.File;
using Entities.Exceptions.File.Minio;
using Entities.UtilityClasses.Minio;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Services.Contracts;


namespace Services
{
    public class FileUploadManager(
        ILoggerService logger,
        IOptions<CustomMinioConfig> minioConfig) : IFileUploadService
    {

        private readonly ILoggerService _logger = logger;
        


        private readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".mp4", ".avi", ".mov", ".wmv", ".mkv"];
        private readonly long MaxFileSize = 5 * 1024 * 1024;
        private readonly CustomMinioConfig minioConfig = minioConfig.Value;




        public async Task<string> Upload(IFormFile File, FolderPaths filePath, string? pastFileName)
        {
            if (!IsValidFileExtension(File.FileName)
                || !IsValidFileSize(File.Length))
                throw new FileBadRequestException();




            string createFileName = String.Concat(Guid.NewGuid().ToString(), Path.GetExtension(File.FileName));
            string fileNameWithPathForMinio = String.Concat(filePath.GetDescription(), createFileName);

            try
            {

                var minio = new MinioClient()
                                    .WithEndpoint(minioConfig.Endpoint)
                                    .WithCredentials(minioConfig.FullAccess.AccessKey, minioConfig.FullAccess.SecretKey)
                                    .WithSSL(minioConfig.SSL)
                                    .Build();
                


                if (pastFileName is not null)
                {
                    await RemoveFile(minio, minioConfig.Bucket, String.Concat(filePath.GetDescription(), pastFileName));
                }



                FileUpload(minio, File, minioConfig.Bucket, fileNameWithPathForMinio).Wait();
            }
            catch (MinioException ex)
            {
                _logger.LogError($"Method: {nameof(Upload)}; FileWithPathForMinio: {fileNameWithPathForMinio}; An error occurred while uploading the image, {ex}");
                throw new MinioGeneralBadRequestException();
            }

            return String.Concat("/", filePath.ToString(), "/", createFileName);
        }


        private async Task FileUpload(IMinioClient minio, IFormFile file, string Bucket, string fileNameWithPath)
        {

            MemoryStream stream = new();
            file.CopyTo(stream);
            stream.Position = 0;

            var contentType = file.ContentType;


            try
            {
                var beArgs = new BucketExistsArgs()
                    .WithBucket(Bucket);
                bool found = await minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    throw new BucketNotFoundException();
                }

                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(Bucket)
                    .WithObject(fileNameWithPath)
                    .WithStreamData(stream)
                    .WithObjectSize(stream.Length)
                    .WithContentType(contentType);
                await minio.PutObjectAsync(putObjectArgs);



            }
            catch (MinioException ex)
            {
                _logger.LogError($"Method: {nameof(FileUpload)}; FileWithPath: {fileNameWithPath}; An error occurred while uploading the image, {ex}");
            }
        }




        public async Task Delete(string filePath, string fileName)
        {
            if (fileName is not null)
            {
                try
                {


                    var minio = new MinioClient()
                                    .WithEndpoint(minioConfig.Endpoint)
                                    .WithCredentials(minioConfig.FullAccess.AccessKey, minioConfig.FullAccess.SecretKey)
                                    .WithSSL(minioConfig.SSL)
                                    .Build();


                    await RemoveFile(minio, minioConfig.Bucket, String.Concat(filePath, fileName));
                }
                catch (MinioException ex)
                {
                    _logger.LogError($"Method: {nameof(Delete)}; FileWithPath: {filePath}; An error occurred while deleting the image, {ex}");
                    throw new MinioGeneralBadRequestException();
                }
            }
        }


        private async Task RemoveFile(IMinioClient minio, string Bucket, string fileWithPath)
        {

            try
            {
                RemoveObjectArgs rmArgs = new RemoveObjectArgs()
                              .WithBucket(Bucket)
                              .WithObject(fileWithPath);
                await minio.RemoveObjectAsync(rmArgs);

            }
            catch (MinioException ex)
            {
                _logger.LogError($"Method: {nameof(RemoveFile)}; FileWithPath: {fileWithPath}; An error occurred while deleting the image, {ex}");
                throw new MinioGeneralBadRequestException();
            }

        }




        #region File Control Funcs

        private bool IsValidFileExtension(string fileName)
        {
            var extension = Path.GetExtension(fileName);
            return AllowedExtensions.Contains(extension.ToLower());
        }


        private bool IsValidFileSize(long fileSize)
        {
            return fileSize <= MaxFileSize;
        }
        #endregion


    }
}
