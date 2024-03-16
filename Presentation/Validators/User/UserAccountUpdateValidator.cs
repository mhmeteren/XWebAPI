

using Entities.DataTransferObjects.User;
using FluentValidation;

namespace Presentation.Validators.User
{
    public class UserAccountUpdateValidator : AbstractValidator<UserDtoForAccountUpdate>
    {
        public UserAccountUpdateValidator()
        {
            RuleFor(x => x.FullName).NotNull().NotEmpty().Length(5, 100);
            RuleFor(x => x.About).NotNull().NotEmpty().Length(5, 200);
            RuleFor(x => x.Location).NotNull().NotEmpty().Length(5, 50);
            RuleFor(x => x.Gender).NotNull().NotEmpty().Length(4, 10).Must(ValidatorHelpers.IsValidGender);
            RuleFor(x => x.Birthday).Must(ValidatorHelpers.IsValidBirthdayDate);
        }
    }
}
