
using Entities.Exceptions.TweetLikes;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class TweetLikesManager(
        IRepositoryManager manager,
        IUserService userService) : ITweetLikesService
    {
        private readonly IUserService _userService = userService;


        protected TweetLikes? tweetLikes;

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
