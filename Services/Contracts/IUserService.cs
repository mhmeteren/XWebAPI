using Entities.DataTransferObjects.User;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Services.Contracts
{
    public interface IUserService
    {
      
        Task<IdentityResult> ReqisterUserAsync(UserDtoForRegister userDto);
        Task<UserProfileDto> GetUserProfileByUsernameAsync(string Username);


        Task UpdateProfileImage(IFormFile profileImage, string Username);
        Task UpdateBackgroundImage(IFormFile profileImage, string Username);


        Task<IdentityResult> UpdateProfile(string Username, UserDtoForAccountUpdate userDto);


        Task<Users> GetUserByIdentityNameCheckAndExistsAsync(string identityName);
        Task<Users> GetUserByIdCheckAndExistsAsync(string userId);

    }
}
