
using Entities.DataTransferObjects.BaseUser;
using FluentValidation;

namespace Presentation.Validators.Authentication
{
    public sealed class TokenRefreshValidator : AbstractValidator<TokenDto>
    {
        public TokenRefreshValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty().NotNull();
            RuleFor(x => x.RefreshToken).NotEmpty().NotNull();
        }
    }
}
