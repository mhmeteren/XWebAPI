namespace Entities.Exceptions.TweetMedias
{
    public class TweetIsAlreadyRetweetedBadRequestException()
    : BadRequestException("Tweet has already been retweeted.")
    {

    }
    
}
