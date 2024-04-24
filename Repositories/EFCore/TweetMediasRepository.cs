
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;

namespace Repositories.EFCore
{
    public class TweetMediasRepository(RepositoryContext context) : RepositoryBase<TweetMedias>(context), ITweetMediasRepository
    {

        public async Task<PagedList<TweetMedias>?> GetAllTweetMediasByTweet(string tweetId, TweetMediasParameters parameters, bool trackChanges)
        {
            var tweets = await FindAll(trackChanges)
                 .Where(f => f.TweetId.Equals(tweetId))
                 .ToListAsync();

            return PagedList<TweetMedias>.ToPagedList(tweets, parameters);
        }

        public async Task<List<TweetMedias>?> GetAllTweetMediasByTweet(string tweetId, bool trackChanges) =>
            await FindAll(trackChanges)
                 .Where(f => f.TweetId.Equals(tweetId))
                 .ToListAsync();

        public void CreateRangeTweetMedias(IEnumerable<TweetMedias> tweetMedias) => CreateRange(tweetMedias);
        public void DeleteRangeTweetMedias(IEnumerable<TweetMedias> tweetMedias) => DeleteRange(tweetMedias);

        public async Task<TweetMedias?> GetTweetMediaById(string Id, bool trackChanges) =>
            await FindByCondition(m => m.Id.Equals(Id), trackChanges)
            .SingleOrDefaultAsync();

    }
}
