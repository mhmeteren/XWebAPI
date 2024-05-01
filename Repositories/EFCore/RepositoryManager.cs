using Repositories.Contracts;

namespace Repositories.EFCore
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _context;
        private readonly Lazy<IFollowsRepository> _followerRepository;
        private readonly Lazy<IBlockedUsersRepository> _blockedUsersRepository;
        private readonly Lazy<ITweetsRepository> _tweetsRepository;
        private readonly Lazy<ITweetMediasRepository> _tweetMediasRepository;
        private readonly Lazy<ITweetLikesRepository> _tweetLikesRepository;



        public RepositoryManager(RepositoryContext context)
        {
            _context = context;
            _followerRepository = new Lazy<IFollowsRepository>(() => new FollowsRepository(_context));
            _blockedUsersRepository = new Lazy<IBlockedUsersRepository>(() => new BlockedUsersRepository(_context));
            _tweetsRepository = new Lazy<ITweetsRepository>(() => new TweetsRepository(_context));
            _tweetMediasRepository = new Lazy<ITweetMediasRepository>(() => new TweetMediasRepository(_context));
            _tweetLikesRepository = new Lazy<ITweetLikesRepository>(() => new TweetLikesRepository(_context));

        }

        public IFollowsRepository Follows => _followerRepository.Value;

        public IBlockedUsersRepository BlockedUsers => _blockedUsersRepository.Value;

        public ITweetsRepository Tweets => _tweetsRepository.Value;

        public ITweetMediasRepository TweetMedias => _tweetMediasRepository.Value;

        public ITweetLikesRepository TweetLikes => _tweetLikesRepository.Value;

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
