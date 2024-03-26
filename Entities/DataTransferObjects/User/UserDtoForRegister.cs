namespace Entities.DataTransferObjects.User
{
    public record UserDtoForRegister(
        string FullName,
        string UserName,
        string Email,
        DateTime? Birthday,
        string Password)
    {
    }

}
