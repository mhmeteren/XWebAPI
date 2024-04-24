using Entities.Models;
using Entities.RequestFeatures;

namespace Repositories.Contracts
{
    public interface ITweetLikesRepository : IRepositoryBase<TweetLikes>
    {
        Task<PagedList<TweetLikes>?> GetAllLikesByTweet(string tweetId, TweetLikesParameters parameters, bool trackChanges);
        Task<PagedList<TweetLikes>?> GetAllLikesByUser(string userId, TweetLikesParameters parameters, bool trackChanges);

        Task<TweetLikes?> GetTweetLikeById(string tweetId, string userId, bool trackChanges);

        void CreateLike(TweetLikes tweetLikes);
        void DeleteLike(TweetLikes tweetLikes);
    }
}
