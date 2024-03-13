
namespace Entities.Exceptions.Follow
{
    public class IsFollowerBadRequestException()
        : BadRequestException("User is already being followed.")
    {
    }
}
