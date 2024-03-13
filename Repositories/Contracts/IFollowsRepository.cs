

using Entities.Models;
using Entities.RequestFeatures;

namespace Repositories.Contracts
{
    public interface IFollowsRepository : IRepositoryBase<Follows>
    {
        

        Task<PagedList<Follows>> GetAllFollowingsAsync(string userId, FollowParameters parameters, bool trackChanges);
        Task<PagedList<Follows>> GetAllFollowersAsync(string userId, FollowParameters parameters, bool trackChanges);

        Task<Follows> CheckUserFollowingAsync(string followerId, string followingId, bool trackChanges);


        void FollowUser(Follows follow); // Create
        void UnFollowUser(Follows follow); // Delete

    }
}
