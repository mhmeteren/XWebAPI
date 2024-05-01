
using Entities.Models;
using Entities.RequestFeatures;

namespace Repositories.Contracts
{
    public interface ITweetMediasRepository : IRepositoryBase<TweetMedias>
    {

        Task<PagedList<TweetMedias>?> GetAllTweetMediasByTweet(string tweetId, TweetMediasParameters parameters, bool trackChanges);
        Task<List<TweetMedias>?> GetAllTweetMediasByTweet(string tweetId, bool trackChanges);

        Task<TweetMedias?> GetTweetMediaById(string Id, bool trackChanges);

        void CreateRangeTweetMedias(IEnumerable<TweetMedias> tweetMedias);
        void DeleteRangeTweetMedias(IEnumerable<TweetMedias> tweetMedias);

    }
}
