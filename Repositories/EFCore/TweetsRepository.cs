
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;

namespace Repositories.EFCore
{
    public class TweetsRepository(RepositoryContext context) : RepositoryBase<Tweets>(context), ITweetsRepository
    {


        #region Main Feed
        public async Task<PagedList<Tweets>> GetAllFollowingTweets(IEnumerable<string> followingUsers, TweetParameters parameters, bool trackChanges)
        {
            var tweets = await FindAll(trackChanges)
                 .Where(t => followingUsers.Contains(t.CreaterUserId)
                 && !t.IsDeleting)
                 .Include(f => f.CreaterUser)
                 .Include(t => t.TweetMedias.Take(3))
                 .OrderByDescending(f => f.CreateDate)
                 .ToListAsync();

            return PagedList<Tweets>.ToPagedList(tweets, parameters);
        }

        #endregion Main Feed



        #region Feed by User
        public async Task<PagedList<Tweets>> GetAllRepliesByTweetAsync(string tweetId, TweetParameters parameters, bool trackChanges)
        {
            var tweets = await FindAll(trackChanges)
                 .Where(f => f.MainTweetID != null 
                 && f.MainTweetID.Equals(tweetId)  
                 && !f.IsRetweet 
                 && !f.IsDeleting)
                 .Include(f => f.CreaterUser)
                 .Include(t => t.TweetMedias.Take(3))
                 .OrderByDescending(f => f.CreateDate)
                 .ToListAsync();

            return PagedList<Tweets>.ToPagedList(tweets, parameters);
        }

        public async Task<PagedList<Tweets>> GetAllQuotesByTweetAsync(string tweetId, TweetParameters parameters, bool trackChanges)
        {
            var tweets = await FindAll(trackChanges)
                 .Where(f => f.MainTweetID != null 
                 && f.MainTweetID.Equals(tweetId) 
                 && f.IsRetweet 
                 && f.Content != null 
                 && !f.IsDeleting)
                 .Include(f => f.CreaterUser)
                 .Include(t => t.TweetMedias.Take(3))
                 .OrderByDescending(f => f.CreateDate)
                 .ToListAsync();

            return PagedList<Tweets>.ToPagedList(tweets, parameters);
        }


        public async Task<PagedList<Tweets>> GetAllRetweetersByTweetAsync(string tweetId, TweetParameters parameters, bool trackChanges)
        {
            var tweets = await FindAll(trackChanges)
                 .Where(f => f.MainTweetID != null
                 && f.MainTweetID.Equals(tweetId)
                 && f.IsRetweet
                 && f.Content == null
                 && !f.IsDeleting)
                 .Include(f => f.CreaterUser)
                 .OrderByDescending(f => f.CreateDate)
                 .ToListAsync();

            return PagedList<Tweets>.ToPagedList(tweets, parameters);
        }


        public async Task<PagedList<Tweets>> GetAllRetweetsWithRepliesByUserAsync(string userId, TweetParameters parameters, bool trackChanges)
        {
            var tweets = await FindAll(trackChanges)
                 .Where(f => f.MainTweetID != null && f.CreaterUserId.Equals(userId) && !f.IsDeleting)
                 .Include(x => x.CreaterUser)
                 .Include(t => t.TweetMedias.Take(3))
                 .OrderByDescending(f => f.CreateDate)
                 .ToListAsync();

            return PagedList<Tweets>.ToPagedList(tweets, parameters);
        }


        public async Task<PagedList<Tweets>> GetAllTweetsByUserAsync(string userId, TweetParameters parameters, bool trackChanges)
        {
            var tweets = await FindAll(trackChanges)
                 .Where(f => f.MainTweetID == null && f.CreaterUserId.Equals(userId) && !f.IsDeleting)
                 .Include(x => x.CreaterUser)
                 .Include(t => t.TweetMedias.Take(3))
                 .OrderByDescending(f => f.CreateDate)
                 .ToListAsync();

            return PagedList<Tweets>.ToPagedList(tweets, parameters);
        }
        
        #endregion Feed by User




        #region Get Single or Default
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

        public async Task<Tweets?> GetRetweetbyMainTweetAndUser(string mainTweetId, string createrUserId, bool trackChanges) =>
            await FindByCondition(t => 
            t.CreaterUserId.Equals(createrUserId) 
            && !t.IsDeleting
            && t.MainTweetID != null
            && t.MainTweetID.Equals(mainTweetId)
            && t.IsRetweet, trackChanges)
                .SingleOrDefaultAsync();
        #endregion Get Single or Default




        #region C_UD
        public void CreateTweet(Tweets tweet) => Create(tweet);
        public void DeleteTweet(Tweets tweet) => Delete(tweet);
        public void UpdateTweet(Tweets tweet) => Update(tweet);
        #endregion C_UD
    }
}
