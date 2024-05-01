
namespace Entities.Exceptions.TweetLikes
{
    public class TweetLikeBadRequestException() 
        : BadRequestException("Tweet has already been liked.")
    {
    }
}
