using Entities.DataTransferObjects.User;
using FluentValidation;

namespace Presentation.Validators
{
    public class UserRegisterValidator : AbstractValidator<UserDtoForRegister>
    {

        public UserRegisterValidator()
        {
            RuleFor(x => x.FullName).NotNull().NotEmpty().Length(5, 100);
            RuleFor(x => x.UserName).NotNull().NotEmpty().Length(5, 100);
            RuleFor(x => x.Email).EmailAddress().NotEmpty();
            RuleFor(x => x.Birthday).Must(i => i == null || ValidatorHelpers.BirthdayDateRange(i));
        }
    }
}
