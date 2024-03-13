
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;

namespace Repositories.EFCore
{
    public class BlockedUsersRepository(RepositoryContext context) : RepositoryBase<BlockedUsers>(context), IBlockedUsersRepository
    {
        public async Task<PagedList<BlockedUsers>> GetAllBlockedUsersAsync(string blockerUserId, BlockedUserParameters parameters, bool trackChanges)
        {
            var blockedUsers = await FindAll(trackChanges)
                 .Include(b => b.BlockedUser)
                 .Where(f => f.BlockerUserId.Equals(blockerUserId))
                 .OrderByDescending(f => f.CreateDate)
                 .ToListAsync();

            return PagedList<BlockedUsers>
                .ToPagedList(blockedUsers, parameters);
        }


        public async Task<BlockedUsers> CheckUserBlockedAsync(string blockerUserId, string blockedUserId, bool trackChanges) =>
            await FindByCondition(b => b.BlockerUserId.Equals(blockerUserId) && b.BlockedUserId.Equals(blockedUserId), trackChanges)
            .SingleOrDefaultAsync();


        public void BlockedUser(BlockedUsers blockedUsers) => Create(blockedUsers);

        public void UnBlockedUser(BlockedUsers blockedUsers) => Delete(blockedUsers);

                
    }
}
