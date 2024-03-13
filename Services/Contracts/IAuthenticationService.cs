

using Entities.DataTransferObjects.BaseUser;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Services.Contracts
{
    public interface IAuthenticationService
    {
        Task<bool> ValidateUser(BaseUserDtoForLogin baseUserDtoForLogin);
        Task<TokenDto> CreateToken(bool populateExp);
        Task<TokenDto> RefreshToken(TokenDto tokenDto);


        Task<IdentityResult> ConfirmEmail(string Token, string Id);
        //Task<IdentityResult> UserEmailUpdate(string currentEmail, BaseUserDtoForEmailUpdate userDto);

        //Task<IdentityResult> UserPasswordUpdate(string currentEmail, BaseUserDtoForPasswordUpdate userDto);


        //Task UserForgotPassword(BaseUserDtoForForgotPassword baseUserDto);

        //Task<IdentityResult> UserResetPassword(BaseUserDtoForResetPassword baseUserDto);


        //Task UserProfileImageUpdate(IFormFile ProfileImage, BaseUser user);

        //Task<bool> UserLogout(BaseUser user);
    }
}
