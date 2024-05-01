using AutoMapper;
using Entities.DataTransferObjects.Tweets;
using Entities.DataTransferObjects.User;
using Entities.Exceptions.TweetLikes;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class TweetLikesManager(
        IRepositoryManager manager,
        IMapper mapper) : ITweetLikesService
    {
        private readonly IMapper _mapper = mapper;
        protected TweetLikes? tweetLikes;

        public async Task<PagedResponse<TweetDtoForDetail>> GetAllLikesByUser(string userId, TweetLikesParameters parameters, bool trackChanges)
        {//[NOTE] read cache
            throw new NotImplementedException();
        }

        public async Task<PagedResponse<UserDtoForTweets>> GetAllLikesByTweet(string tweetId, TweetLikesParameters parameters, bool trackChanges)
        {

            var likeUsersWithMetaData = await manager.TweetLikes.GetAllLikesByTweet(tweetId, parameters, trackChanges);

            return new PagedResponse<UserDtoForTweets>(
                items: _mapper.Map<IEnumerable<UserDtoForTweets>>(likeUsersWithMetaData),
                metaData: likeUsersWithMetaData.MetaData);
        }

        public async Task CreateLike(string tweetId, string loggedInUserId)
        {
            if (await IsLiked(tweetId, loggedInUserId))
                throw new TweetLikeBadRequestException();

            manager.TweetLikes.CreateLike(new()
            {
                TweetId = tweetId,
                UserId = loggedInUserId
            });

            await manager.SaveAsync();
        }


        public async Task DeleteLike(string tweetId, string loggedInUserId)
        {
            if (!await IsLiked(tweetId, loggedInUserId))
                throw new TweetDeleteLikeBadRequestException();

            manager.TweetLikes.DeleteLike(tweetLikes);
            await manager.SaveAsync();
        }


        private async Task<bool> IsLiked(string tweetId, string UserId)
        {
            tweetLikes = await GetTweetLikesbyId(tweetId, UserId, false);
            return tweetLikes != null;
        }

        private async Task<TweetLikes?> GetTweetLikesbyId(string tweetId, string UserId, bool trackChanges) =>
            await manager.TweetLikes.GetTweetLikeById(tweetId, UserId, trackChanges);


    }
}
