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

        readonly private Lazy<ITweetsService> _tweetsService;
        readonly private Lazy<ITweetMediasService> _tweetMediasService;
        readonly private Lazy<ITweetLikesService> _tweetLikesService;



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

            _tweetMediasService = new(() => new TweetMediasManager(repositoryManager, fileUploadService));
            _tweetLikesService = new(() => new TweetLikesManager(repositoryManager, mapper));
            _tweetsService = new(() => new TweetsManager(repositoryManager, mapper, _userService.Value, _tweetMediasService.Value, _tweetLikesService.Value, _followerService.Value));

        }



        public IUserService UserService => _userService.Value;

        public IAuthenticationService AuthenticationService => _authenticationService.Value;

        public IFollowsService FollowsService => _followerService.Value;

        public IBlockedUsersService BlockedUsersService => _blockedUsersService.Value;

        public ITweetsService TweetsService => _tweetsService.Value;

        public ITweetMediasService TweetMediasService => _tweetMediasService.Value;

        public ITweetLikesService TweetLikesService => _tweetLikesService.Value;
    }
}
