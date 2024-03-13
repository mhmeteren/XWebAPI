

namespace Entities.Exceptions.Block
{
    public class UnBlockedBadRequestException()
    : BadRequestException("User is not already blocked.")
    {
    }

}
