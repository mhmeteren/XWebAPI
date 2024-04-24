
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;

namespace Repositories.EFCore
{
    public class TweetLikesRepository(RepositoryContext context) : RepositoryBase<TweetLikes>(context), ITweetLikesRepository
    {
        public async Task<PagedList<TweetLikes>?> GetAllLikesByTweet(string tweetId, TweetLikesParameters parameters, bool trackChanges)
        {
            var tweetLikes = await FindAll(trackChanges)
                 .Where(x => x.TweetId.Equals(tweetId))
                 .Include(x => x.User)
                 .OrderByDescending(x => x.CreateDate)
                 .ToListAsync();

            return PagedList<TweetLikes>.ToPagedList(tweetLikes, parameters);
        }

        public async Task<PagedList<TweetLikes>?> GetAllLikesByUser(string userId, TweetLikesParameters parameters, bool trackChanges)
        {
            var tweetLikes = await FindAll(trackChanges)
                 .Where(x => x.UserId.Equals(userId))
                 .Include(x => x.Tweets)
                 .OrderByDescending(x => x.CreateDate)
                 .ToListAsync();

            return PagedList<TweetLikes>.ToPagedList(tweetLikes, parameters);
        }
     
        public async Task<TweetLikes?> GetTweetLikeById(string tweetId, string userId, bool trackChanges) =>
            await FindByCondition(x => x.TweetId.Equals(tweetId) && x.UserId.Equals(userId), trackChanges)
            .SingleOrDefaultAsync();

        public void CreateLike(TweetLikes tweetLikes) => Create(tweetLikes);
        public void DeleteLike(TweetLikes tweetLikes) => Delete(tweetLikes);

    }
}
