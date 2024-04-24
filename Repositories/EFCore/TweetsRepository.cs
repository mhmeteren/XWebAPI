
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;

namespace Repositories.EFCore
{
    public class TweetsRepository(RepositoryContext context) : RepositoryBase<Tweets>(context), ITweetsRepository
    {


        public async Task<PagedList<Tweets>> GetAllCommentTweetsByTweetAsync(string tweetId, TweetParameters parameters, bool trackChanges)
        {
            var tweets = await FindAll(trackChanges)
                 .Where(f => f.MainTweetID != null && f.MainTweetID.Equals(tweetId)  && !f.IsRetweet && !f.IsDeleting)
                 .OrderByDescending(f => f.CreateDate)
                 .ToListAsync();

            return PagedList<Tweets>.ToPagedList(tweets, parameters);
        }

        public async Task<PagedList<Tweets>> GetAllReTweetsAndCommentTweetsByUserAsync(string userId, TweetParameters parameters, bool trackChanges)
        {
            var tweets = await FindAll(trackChanges)
                 .Where(f => f.MainTweetID != null && f.CreaterUserId.Equals(userId) && !f.IsDeleting)
                 .OrderByDescending(f => f.CreateDate)
                 .ToListAsync();

            return PagedList<Tweets>.ToPagedList(tweets, parameters);
        }


        public async Task<PagedList<Tweets>> GetAllTweetsByUserAsync(string userId, TweetParameters parameters, bool trackChanges)
        {
            var tweets = await FindAll(trackChanges)
                 .Where(f => f.MainTweetID == null && f.CreaterUserId.Equals(userId) && !f.IsDeleting)
                 .OrderByDescending(f => f.CreateDate)
                 .ToListAsync();

            return PagedList<Tweets>.ToPagedList(tweets, parameters);
        }


        public async Task<PagedList<Tweets>> GetAllLikedTweetsByUserAsync(string userId, TweetParameters parameters, bool trackChanges)
        {
            throw new NotImplementedException();
        }


        public async Task<Tweets?> GetTweetDetailbyId(string tweetId, bool trackChanges)
        {
            var tweet = await FindByCondition(t => t.Id.Equals(tweetId) && !t.IsDeleting, trackChanges)
                .Include(t => t.CreaterUser)
                .Include(t => t.TweetMedias.Take(3))
                .SingleOrDefaultAsync();

            if (tweet != null && tweet.IsRetweet && !string.IsNullOrEmpty(tweet.MainTweetID))
            {
                var relatedTweet = await FindByCondition(t => t.Id.Equals(tweet.MainTweetID) && !t.IsDeleting, trackChanges)
                    .Include(t => t.CreaterUser)
                    .Include(t => t.TweetMedias.Take(3))
                    .SingleOrDefaultAsync();

              tweet.MainTweet = relatedTweet;
            }

            return tweet;
        }

        public async Task<Tweets?> GetTweetbyId(string tweetId, bool trackChanges) =>
            await FindByCondition(t => t.Id.Equals(tweetId) && !t.IsDeleting, trackChanges)
                .SingleOrDefaultAsync();

        public async Task<Tweets?> GetTweetbyIdAndUser(string tweetId, string createrUserId, bool trackChanges) =>
            await FindByCondition(t => t.Id.Equals(tweetId) && t.CreaterUserId.Equals(createrUserId) && !t.IsDeleting, trackChanges)
                .SingleOrDefaultAsync();

        public void CreateTweet(Tweets tweet) => Create(tweet);
        public void DeleteTweet(Tweets tweet) => Delete(tweet);
        public void UpdateTweet(Tweets tweet) => Update(tweet);

    }
}
