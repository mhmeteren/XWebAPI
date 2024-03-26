
using Entities.DataTransferObjects.BaseUser;
using FluentValidation;

namespace Presentation.Validators.Authentication
{
    public sealed class LoginValidator : AbstractValidator<BaseUserDtoForLogin>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Username).NotNull().NotEmpty().Length(5, 100).Must(i => !i.Contains(' ')).WithMessage("Invalid username");
            RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(8);
        }
    }
}
