

namespace Entities.Exceptions.GeneralExceptions
{
    public class GeneralBadRequestException(string message) : BadRequestException(message)
    {
    }
}
