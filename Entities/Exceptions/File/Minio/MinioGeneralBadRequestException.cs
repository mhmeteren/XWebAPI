
namespace Entities.Exceptions.File.Minio
{
    public sealed class MinioGeneralBadRequestException()
        : BadRequestException("An error occurred while updating the media. Please try again later.")
    {
    }
}
