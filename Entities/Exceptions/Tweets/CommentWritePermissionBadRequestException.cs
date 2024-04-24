
namespace Entities.Exceptions.Tweets
{
    public class CommentWritePermissionBadRequestException() 
        : BadRequestException("User does not have permission to write a comment on this tweet.")
    {
    }
}
