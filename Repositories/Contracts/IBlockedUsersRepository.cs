
using Entities.Models;
using Entities.RequestFeatures;

namespace Repositories.Contracts
{
    public interface IBlockedUsersRepository : IRepositoryBase<BlockedUsers>
    {

        Task<PagedList<BlockedUsers>> GetAllBlockedUsersAsync(string blockerUserId, BlockedUserParameters parameters, bool trackChanges);

        Task<BlockedUsers> CheckUserBlockedAsync(string blockerUserId, string blockedUserId, bool trackChanges);

        void BlockedUser(BlockedUsers blockedUsers); 
        void UnBlockedUser(BlockedUsers blockedUsers);

    }
}
