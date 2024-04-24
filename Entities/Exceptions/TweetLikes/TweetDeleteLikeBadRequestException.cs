
namespace Entities.Exceptions.TweetLikes
{
    public class TweetDeleteLikeBadRequestException()
    : BadRequestException("Tweet has not been liked before.")
    {
    }
}
