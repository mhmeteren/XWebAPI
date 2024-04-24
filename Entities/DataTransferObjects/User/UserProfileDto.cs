
namespace Entities.DataTransferObjects.User
{
    public record UserProfileDto(
        string? Id,
        string? FullName,
        string? UserName,
        string? ProfileImageUrl,
        string? BackgroundImageUrl,
        string? About,
        string? Location,
        bool IsPrivateAccount,
        bool IsVerifiedAccount)
    {

    }

}
