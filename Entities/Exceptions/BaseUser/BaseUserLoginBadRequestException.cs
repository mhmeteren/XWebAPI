
namespace Entities.Exceptions.BaseUser
{
    public class BaseUserLoginBadRequestException() : BadRequestException("Invalid Username or password.")
    {
    }
}
