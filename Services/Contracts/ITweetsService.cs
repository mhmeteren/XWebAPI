
using Entities.DataTransferObjects.Tweets;
using Entities.DataTransferObjects.User;
using Entities.Enums;
using Entities.Models;
using Entities.RequestFeatures;

namespace Services.Contracts
{
    public interface ITweetsService
    {

        Task<PagedResponse<TweetDtoForDetail>> GetAllFollowingTweetsByUserAsync(string loggedInUsername, TweetParameters parameters, bool trackChanges);


        Task<PagedResponse<TweetDtoForDetail>> GetAllTweetsByUserAsync(string username, string loggedInUsername, UserFeedType userFeedType, TweetParameters parameters, bool trackChanges);
        Task<PagedResponse<TweetDtoForDetail>> GetAllTweetsByTweetAsync(string tweetId, string loggedInUsername, TweetType tweetType, TweetParameters parameters, bool trackChanges);
        Task<PagedResponse<UserDtoForTweets>> GetAllRetweetersByTweetAsync(string tweetId, string loggedInUsername, TweetParameters parameters, bool trackChanges);
        Task<PagedResponse<UserDtoForTweets>> GetAllLikeUsersByTweetAsync(string tweetId, string loggedInUsername, TweetLikesParameters parameters, bool trackChanges);


        Task<TweetDtoForDetail> GetTweetById(string tweetId, string loggedInUsername, bool trackChanges);
        Task<Tweets> GetTweetByIdCheckAndExits(string tweetId, bool trackChanges);
        Task<Tweets> GetTweetByIdAndUserCheckAndExits(string tweetId, string createrUserId, bool trackChanges);
        Task<string> CheckTweetMediaViewPermission(string mediaId, string loggedInUsername);


        Task CreateTweet(string loggedInUsername, TweetDtoForCreate tweetDto);
        Task EditTweet(string tweetId, string loggedInUsername, TweetDtoForEdit tweetDto);
        Task DeleteTweet(string tweetId, string loggedInUsername);


        Task CreateTweetLike(string tweetId, string loggedInUsername);
        Task DeleteTweetLike(string tweetId, string loggedInUsername);




    }
}
