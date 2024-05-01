
using Entities.Models;
using Entities.RequestFeatures;

namespace Repositories.Contracts
{
    public interface ITweetsRepository : IRepositoryBase<Tweets>
    {

        Task<PagedList<Tweets>> GetAllFollowingTweets(IEnumerable<string> followingUsers, TweetParameters parameters, bool trackChanges);



        Task<PagedList<Tweets>> GetAllTweetsByUserAsync(string userId, TweetParameters parameters, bool trackChanges);
        Task<PagedList<Tweets>> GetAllRetweetsWithRepliesByUserAsync(string userId, TweetParameters parameters, bool trackChanges); 
      
        Task<PagedList<Tweets>> GetAllRepliesByTweetAsync(string tweetId, TweetParameters parameters, bool trackChanges);
        Task<PagedList<Tweets>> GetAllQuotesByTweetAsync(string tweetId, TweetParameters parameters, bool trackChanges);
        Task<PagedList<Tweets>> GetAllRetweetersByTweetAsync(string tweetId, TweetParameters parameters, bool trackChanges);



        Task<Tweets?> GetTweetDetailbyId(string tweetId, bool trackChanges);
        Task<Tweets?> GetTweetbyId(string tweetId, bool trackChanges);
        Task<Tweets?> GetTweetbyIdAndUser(string tweetId, string createrUserId, bool trackChanges);
        Task<Tweets?> GetRetweetbyMainTweetAndUser(string mainTweetId, string createrUserId, bool trackChanges);



        void CreateTweet(Tweets tweet);
        void UpdateTweet(Tweets tweet);
        void DeleteTweet(Tweets tweet);

    }
}
