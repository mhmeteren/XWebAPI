using AutoMapper;
using Entities.DataTransferObjects.User;
using Entities.Enums;
using Entities.Exceptions.BaseUser;
using Entities.Exceptions.File;
using Entities.Exceptions.GeneralExceptions;
using Entities.Exceptions.User;
using Entities.Models;
using Entities.UtilityClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class UsersManager(
        IRepositoryManager manager,
        IMapper mapper,
        UserManager<BaseUser> baseUserManager,
        IFileUploadService fileUploadService,
        IValidatorService validatorService) : IUserService
    {
        private readonly IRepositoryManager _manager = manager;
        private readonly IMapper _mapper = mapper;
        private readonly IFileUploadService _fileUploadService = fileUploadService;

        private readonly UserManager<BaseUser> _baseUserManager = baseUserManager;




        public async Task<UserProfileDto> GetUserProfileByUsernameAsync(string Username) =>
            _mapper.Map<UserProfileDto>(await GetUserByUsernameAsync(Username));


        public async Task<IdentityResult> ReqisterUserAsync(UserDtoForRegister userDto)
        {
            var user = _mapper.Map<Users>(userDto);
            var creationResult = await _baseUserManager.CreateAsync(user, userDto.Password);

            if (creationResult.Succeeded)
            {
                var roleBindingResult = await _baseUserManager.AddToRolesAsync(user, [Roles.User.CustomToUpper()]);
                if (roleBindingResult.Succeeded)
                {
                    //send confirmation mail
                }
                else
                {
                    throw new UserGeneralBadRequestException();
                    // add specific err log
                }
            }

            return creationResult;
        }




        public async Task UpdateBackgroundImage(IFormFile profileImage, string Username)
        {
            Users user = await GetUserByUsernameAsync(Username);

            user.BackgroundImageUrl = await _fileUploadService
                .Upload(profileImage,
                FolderPaths.UsersBackgroundImages,
                ImageUrlToImageName(user.BackgroundImageUrl));

            var result = await _baseUserManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new FileUploadGeneralBadRequestException();

        }


        public async Task UpdateProfileImage(IFormFile profileImage, string Username)
        {

            Users user = await GetUserByUsernameAsync(Username);

            user.ProfileImageUrl = await _fileUploadService
                .Upload(profileImage,
                FolderPaths.UsersProfileImages,
                ImageUrlToImageName(user.ProfileImageUrl));

            var result = await _baseUserManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new FileUploadGeneralBadRequestException();

        }


        private static string? ImageUrlToImageName(string ImageFullPath)
        {
            var tmp = string.IsNullOrWhiteSpace(ImageFullPath) ? null : ImageFullPath.Split("/");
            return tmp?[^1];
        }




        public async Task<IdentityResult> UpdateProfile(string Username, UserDtoForAccountUpdate userDto)
        {
            Users user = await GetUserByUsernameAsync(Username);
            _mapper.Map(userDto, user);

            var result = await _baseUserManager.UpdateAsync(user);

            return result;
        }



        public async Task<Users> GetUserByIdentityNameCheckAndExistsAsync(string identityName) =>
            await GetUserByUsernameAsync(identityName);



        private async Task<Users> GetUserByUsernameAsync(string Username)
        {
            if (!validatorService.IsValidUsername(Username))
                throw new UserInvalidValueBadRequestException("Username");


            return await _baseUserManager.FindByNameAsync(Username) is not Users user ?
                    throw new UserNotFoundException()
                    : user;
        }




        public async Task<Users> GetUserByIdCheckAndExistsAsync(string userId)
        {
            return await _baseUserManager.FindByIdAsync(userId) is not Users user ?
                    throw new UserNotFoundException()
                    : user;
        }

    }
}
