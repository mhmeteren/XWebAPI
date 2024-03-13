
namespace Entities.Exceptions.File.Minio
{
    public sealed class MinioGeneralBadRequestException()
        : BadRequestException("An error occurred while updating the image. Please try again later.")
    {
    }
}
