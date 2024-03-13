using Services.Contracts;

namespace Services
{
    public class ValidatorManager : IValidatorService
    {
        public bool IsValidUsername(string? Username)
        {
            return (!string.IsNullOrEmpty(Username)
                && !Username.Contains(' ')
                && 5 <= Username.Length 
                && Username.Length <= 100);
        }
    }
}
