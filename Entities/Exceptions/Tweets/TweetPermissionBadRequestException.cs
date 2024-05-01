
namespace Entities.Exceptions.Tweets
{
    public class TweetPermissionBadRequestException()
    : BadRequestException("User does not have permission on this tweet.")
    {
    }
}
