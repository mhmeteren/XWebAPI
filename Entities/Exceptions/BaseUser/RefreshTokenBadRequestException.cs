namespace Entities.Exceptions.BaseUser
{
    public class RefreshTokenBadRequestException()
        : BadRequestException("Invalid client request. The token has some invalid values.")
    {
    }

}
