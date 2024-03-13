

namespace Entities.Exceptions.Block
{
    public class IsBlockedBadRequestException()
        : BadRequestException("User is already being blocked.")
    {
    }

}
