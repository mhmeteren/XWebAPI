using Entities.DataTransferObjects.Follower;
using Entities.Models;
using Entities.RequestFeatures;

namespace Services.Contracts
{
    public interface IFollowsService
    {
        Task<PagedResponse<FollowingDto>> GetAllFollowingsAsync(string username, string loggedInUsername, FollowParameters parameters, bool trackChanges);
        Task<PagedResponse<FollowerDto>> GetAllFollowersAsync(string username, string loggedInUsername, FollowParameters parameters, bool trackChanges);


        
        Task<bool> IsFollower(string followerId, string followingId);

        Task<Follows> CheckFollowerAsync(string followerId, string followingId);


        Task UserFollow(string userIdentityName, string followingUserId);
        Task UserUnFollow(string userIdentityName, string followingUserId);
        Task DeleteFollower(string userIdentityName, string followerId);

       

    }
}
