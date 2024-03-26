
namespace Entities.DataTransferObjects.BaseUser
{
    public record TokenDto(string? AccessToken, string? RefreshToken)
    {
    }
}
