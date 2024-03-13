
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;

namespace Repositories.EFCore
{
    public class FollowsRepository(RepositoryContext context) : RepositoryBase<Follows>(context), IFollowsRepository
    {


        public async Task<PagedList<Follows>> GetAllFollowersAsync(string userId, FollowParameters parameters, bool trackChanges)
        {
           var followers = await FindAll(trackChanges)
                .Include(f => f.FollowerUser)
                .Where(f => f.FollowingId.Equals(userId))
                .OrderByDescending(f => f.CreateDate)
                .ToListAsync();

            return PagedList<Follows>
                .ToPagedList(followers, parameters);
        }

        public async Task<PagedList<Follows>> GetAllFollowingsAsync(string userId, FollowParameters parameters, bool trackChanges)
        {
            var followers = await FindAll(trackChanges)
                 .Include(f => f.FollowingUser)
                 .Where(f => f.FollowerId.Equals(userId))
                 .OrderByDescending(f => f.CreateDate)
                 .ToListAsync();

            return PagedList<Follows>
                .ToPagedList(followers, parameters);
        }


        public async Task<Follows> CheckUserFollowingAsync(string followerId, string followingId, bool trackChanges) =>
            await FindByCondition(f => f.FollowerId.Equals(followerId) && f.FollowingId.Equals(followingId), trackChanges)
                .SingleOrDefaultAsync();


        public void FollowUser(Follows follow) => Create(follow);

        public void UnFollowUser(Follows follow) => Delete(follow);

    }
}
