using AutoMapper;
using Entities.DataTransferObjects.BlockedUsers;
using Entities.Exceptions.Block;
using Entities.Exceptions.GeneralExceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Services.Contracts;


namespace Services
{
    public class BlockedUsersManager(
        IRepositoryManager manager,
        IMapper mapper,
        IUserService userService) : IBlockedUsersService
    {
        private readonly IRepositoryManager _manager = manager;
        private readonly IMapper _mapper = mapper;
        private readonly IUserService _userService = userService;

        public async Task<PagedResponse<BlockedUserDto>> GetAllBlockedUsersByBlockerUserIdAsync(string IdentityUserName, BlockedUserParameters parameters, bool trackChanges)
        {
            var blockerUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(IdentityUserName);

            var blockedUsersWithMetaData = await _manager.BlockedUsers.GetAllBlockedUsersAsync(blockerUser.Id, parameters, trackChanges);
           
            return new PagedResponse<BlockedUserDto>(
                items: _mapper.Map<IEnumerable<BlockedUserDto>>(blockedUsersWithMetaData),
                metaData: blockedUsersWithMetaData.MetaData);
    
        }

        public async Task BlockedUser(string userIdentityName, string blockedUserId)
        {
            var blockerUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(userIdentityName);
            if (blockerUser.Id.Equals(blockedUserId))
                throw new BlockGeneralBadRequestException();


            var blockedUser = await _userService.GetUserByIdCheckAndExistsAsync(blockedUserId);
            if (await IsBlocked(blockerUser.Id, blockedUser.Id))
                throw new IsBlockedBadRequestException();


            await DeleteAllFollowsForBlocking(blockerUser.Id, blockedUser.Id);

            _manager.BlockedUsers.BlockedUser(new() { BlockerUserId = blockerUser.Id, BlockedUserId = blockedUser.Id });
            await _manager.SaveAsync();
        }


        public async Task DeleteAllFollowsForBlocking(string blockerUserId, string blockedUserId)
        {
            //Users already is checked!!!.
            var following = await _manager
                .Follows
                .CheckUserFollowingAsync(blockerUserId, blockedUserId, false);

            if (following is not null)
                _manager.Follows.UnFollowUser(following);

            var follower = await _manager
                    .Follows
                    .CheckUserFollowingAsync(blockedUserId, blockerUserId, false);

            if (follower is not null)
                _manager.Follows.UnFollowUser(follower);
        }



        public async Task UnBlockedUser(string userIdentityName, string blockedUserId)
        {
            var blockerUser = await _userService.GetUserByIdentityNameCheckAndExistsAsync(userIdentityName);
            if (blockerUser.Id.Equals(blockedUserId))
                throw new BlockGeneralBadRequestException();


            var blockedUser = await _userService.GetUserByIdCheckAndExistsAsync(blockedUserId);
            _manager.BlockedUsers.UnBlockedUser(await CheckUserBlockedAsync(blockerUser.Id, blockedUser.Id, false));
            await _manager.SaveAsync();
        }


        public async Task<bool> IsBlocked(string blockerUserId, string blockedUserId)
        {
           return await _manager
                .BlockedUsers
                .CheckUserBlockedAsync(blockerUserId, blockedUserId, false) is not null;
        }

        public async Task<BlockedUsers> CheckUserBlockedAsync(string blockerUserId, string blockedUserId, bool trackChanges)
        {
            return await _manager
                .BlockedUsers
                .CheckUserBlockedAsync(blockerUserId, blockedUserId, trackChanges) ?? throw new UnBlockedBadRequestException();
        }
    }
}
