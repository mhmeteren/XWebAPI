
using Entities.DataTransferObjects.Tweets;
using Entities.Models;

namespace Services.Contracts
{
    public interface ITweetsService
    {
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
