namespace Entities.DataTransferObjects.User
{
    public record UserDtoForAccountUpdate(
        string? FullName, 
        string? About, 
        string? Location,
        string? Gender,
        DateTime? Birthday)
    {


    }

}
