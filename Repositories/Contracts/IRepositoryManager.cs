
namespace Repositories.Contracts
{
    public interface IRepositoryManager
    {
        IFollowsRepository Follows { get; }
        IBlockedUsersRepository BlockedUsers { get; }

        ITweetsRepository Tweets { get; }
        ITweetMediasRepository TweetMedias { get; }
        ITweetLikesRepository TweetLikes { get; }
        Task SaveAsync();
    }
}
