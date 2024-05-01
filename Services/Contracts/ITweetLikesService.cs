using Entities.DataTransferObjects.Tweets;
using Entities.DataTransferObjects.User;
using Entities.RequestFeatures;

namespace Services.Contracts
{
    public interface ITweetLikesService
    {

        Task<PagedResponse<TweetDtoForDetail>> GetAllLikesByUser(string userId, TweetLikesParameters parameters, bool trackChanges);
        Task<PagedResponse<UserDtoForTweets>> GetAllLikesByTweet(string tweetId, TweetLikesParameters parameters, bool trackChanges);
        Task CreateLike(string tweetId, string loggedInUserId);
        Task DeleteLike(string tweetId, string loggedInUserId);
    }
}
