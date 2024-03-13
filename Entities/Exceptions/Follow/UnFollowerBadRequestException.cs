
namespace Entities.Exceptions.Follow
{
    public class UnFollowerBadRequestException()
    : BadRequestException("User is not already being followed.")
    {
    }

}
