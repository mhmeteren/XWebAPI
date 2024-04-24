
using Entities.Models;
using Entities.RequestFeatures;

namespace Repositories.Contracts
{
    public interface ITweetsRepository : IRepositoryBase<Tweets>
    {
        Task<PagedList<Tweets>> GetAllTweetsByUserAsync(string userId, TweetParameters parameters, bool trackChanges);
        Task<PagedList<Tweets>> GetAllReTweetsAndCommentTweetsByUserAsync(string userId, TweetParameters parameters, bool trackChanges);
        Task<PagedList<Tweets>> GetAllLikedTweetsByUserAsync(string userId, TweetParameters parameters, bool trackChanges);
        Task<PagedList<Tweets>> GetAllCommentTweetsByTweetAsync(string tweetId, TweetParameters parameters, bool trackChanges);

        Task<Tweets?> GetTweetDetailbyId(string tweetId, bool trackChanges);
        Task<Tweets?> GetTweetbyId(string tweetId, bool trackChanges);
        Task<Tweets?> GetTweetbyIdAndUser(string tweetId, string createrUserId, bool trackChanges);

        void CreateTweet(Tweets tweet);
        void UpdateTweet(Tweets tweet);
        void DeleteTweet(Tweets tweet);

    }
}
