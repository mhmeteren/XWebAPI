
using Entities.DataTransferObjects.BlockedUsers;
using Entities.Models;
using Entities.RequestFeatures;

namespace Services.Contracts
{
    public interface IBlockedUsersService
    {
        Task<PagedResponse<BlockedUserDto>> GetAllBlockedUsersByBlockerUserIdAsync(string IdentityUserName, BlockedUserParameters parameters, bool trackChanges);

        Task<bool> IsBlocked(string blockerUserId, string blockedUserId);
        Task<BlockedUsers> CheckUserBlockedAsync(string blockerUserId, string blockedUserId, bool trackChanges);

        Task BlockedUser(string userIdentityName, string blockedUserId);
        Task UnBlockedUser(string userIdentityName, string blockedUserId);
    }
}
