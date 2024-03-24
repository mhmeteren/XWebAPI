
using AutoMapper;
using Entities.DataTransferObjects.Follower;
using Entities.Exceptions.Follow;
using Entities.Exceptions.GeneralExceptions;
using Entities.Exceptions.User;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class FollowsManager(
        IRepositoryManager manager,
        IMapper mapper,
        IUserService userService,
        IBlockedUsersService blockedUsersService)
        : IFollowsService
    {
        private readonly IRepositoryManager _manager = manager;
        private readonly IMapper _mapper = mapper;
        private readonly IUserService _userService = userService;
        private readonly IBlockedUsersService blockedUsersService = blockedUsersService;

        public async Task<PagedResponse<FollowerDto>> GetAllFollowersAsync(string username, string loggedInUsername, FollowParameters parameters, bool trackChanges)
        {
            var user = await GetUserAndCheckAccountPermission(username, loggedInUsername);

            var followerWithMetaData = await _manager.Follows.GetAllFollowersAsync(user.Id, parameters, trackChanges);

            return new PagedResponse<FollowerDto>(
                items: _mapper.Map<IEnumerable<FollowerDto>>(followerWithMetaData),
                metaData: followerWithMetaData.MetaData);
        }


        public async Task<PagedResponse<FollowingDto>> GetAllFollowingsAsync(string username, string loggedInUsername, FollowParameters parameters, bool trackChanges)
        {
            var user = await GetUserAndCheckAccountPermission(username, loggedInUsername);

            var followingsWithMetaData = await _manager.Follows.GetAllFollowingsAsync(user.Id, parameters, trackChanges);


            return new PagedResponse<FollowingDto>(
                items: _mapper.Map<IEnumerable<FollowingDto>>(followingsWithMetaData),
                metaData: followingsWithMetaData.MetaData);
        }


        private async Task<Users> GetUserAndCheckAccountPermission(string username, string loggedInUsername)
        {
            var user = await _userService.GetUserByIdentityNameCheckAndExistsAsync(username);

            if (!user.UserName.Equals(loggedInUsername))
            {
                var loggedInUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(loggedInUsername);

                if (user.IsPrivateAccount && !await IsFollower(loggedInUser.Id, user.Id))
                    throw new UserAccountIsPrivateBadRequestException();

            }

            return user;
        }



        public async Task UserFollow(string userIdentityName, string followingUserId)
        {

            var followerUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(userIdentityName);
            if (followerUser.Id.Equals(followingUserId))
                throw new FollowGeneralBadRequestException();

            var followingUser = await _userService.GetUserByIdCheckAndExistsAsync(followingUserId);

            if (await blockedUsersService.IsBlocked(followingUser.Id, followerUser.Id)
                || await blockedUsersService.IsBlocked(followerUser.Id, followingUser.Id))
                throw new FollowGeneralBadRequestException();


            if (await IsFollower(followerUser.Id, followingUser.Id))
                throw new IsFollowerBadRequestException();


            _manager.Follows.FollowUser(
                new()
                {
                    FollowerId = followerUser.Id,
                    FollowingId = followingUser.Id
                });

            await _manager.SaveAsync();
        }

        public async Task<bool> IsFollower(string followerId, string followingId)
        {
            return await _manager.Follows.CheckUserFollowingAsync(followerId, followingId, false) is not null;
        }




        public async Task UserUnFollow(string userIdentityName, string followingUserId)
        {
            var followerUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(userIdentityName);
            if (followerUser.Id.Equals(followingUserId))
                throw new FollowGeneralBadRequestException();

            var follow = await CheckFollowerAsync(followerUser.Id, followingUserId);

            _manager.Follows.UnFollowUser(follow);
            await _manager.SaveAsync();
        }

        public async Task DeleteFollower(string userIdentityName, string followerId)
        {
            var mainUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(userIdentityName);
            if (mainUser.Id.Equals(followerId))
                throw new FollowGeneralBadRequestException();

            var follow = await CheckFollowerAsync(followerId, mainUser.Id);

            _manager.Follows.UnFollowUser(follow);
            await _manager.SaveAsync();
        }

        public async Task<Follows> CheckFollowerAsync(string followerId, string followingId)
        {
            return await _manager
                .Follows
                .CheckUserFollowingAsync(followerId, followingId, false) ?? throw new UnFollowerBadRequestException();
        }

    }
}
