using Repositories.Contracts;

namespace Repositories.EFCore
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _context;
        private readonly Lazy<IUsersRepository> _userRepository;
        private readonly Lazy<IFollowsRepository> _followerRepository;
        private readonly Lazy<IBlockedUsersRepository> _blockedUsersRepository;



        public RepositoryManager(RepositoryContext context)
        {
            _context = context;
            _userRepository = new Lazy<IUsersRepository>(() => new UsersRepository(_context));
            _followerRepository = new Lazy<IFollowsRepository>(() => new FollowsRepository(_context));
            _blockedUsersRepository = new Lazy<IBlockedUsersRepository>(() => new BlockedUsersRepository(_context));

        }

        public IUsersRepository Users => _userRepository.Value;

        public IFollowsRepository Follows => _followerRepository.Value;

        public IBlockedUsersRepository BlockedUsers => _blockedUsersRepository.Value;

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
