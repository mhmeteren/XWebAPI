

namespace Entities.Exceptions.GeneralExceptions
{
    public class FollowGeneralBadRequestException()
        : BadRequestException("The user cannot be followed or unfollowed.")
    {
    }
}
