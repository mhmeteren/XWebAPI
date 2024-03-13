using AutoMapper;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Repositories.Contracts;
using Services.Contracts;


namespace Services
{
    public class ServiceManager : IServiceManager
    {


        readonly private Lazy<IUserService> _userService;

        readonly private Lazy<IAuthenticationService> _authenticationService;

        readonly private Lazy<IFollowsService> _followerService;
        readonly private Lazy<IBlockedUsersService> _blockedUsersService;



        public ServiceManager(
        IRepositoryManager repositoryManager,
        IMapper mapper,
        ICacheService cacheService,
        IConfiguration config,
        IFileUploadService fileUploadService,
        IValidatorService validatorService,
        UserManager<BaseUser> baseUserManager)
        {
            _userService = new(() => new UsersManager(repositoryManager, mapper, baseUserManager, fileUploadService, validatorService));

            _authenticationService = new(() => new AuthenticationManager(cacheService, baseUserManager, config));

            _blockedUsersService = new(() => new BlockedUsersManager(repositoryManager, mapper, _userService.Value));
            _followerService = new(() => new FollowsManager(repositoryManager, mapper, _userService.Value, _blockedUsersService.Value));

        }



        public IUserService UserService => _userService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;

        public IFollowsService FollowsService => _followerService.Value;

        public IBlockedUsersService BlockedUsersService => _blockedUsersService.Value;
    }
}
