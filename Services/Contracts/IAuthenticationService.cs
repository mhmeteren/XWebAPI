

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
    }
}
