namespace Entities.Exceptions.User
{
    public class UserInvalidValueBadRequestException : BadRequestException
    {
        public UserInvalidValueBadRequestException() : base("Invalid Value.")
        {
            
        }

        public UserInvalidValueBadRequestException(string objectName) : base($"Invalid {objectName} Value.")
        {
            
        }
    }
}
