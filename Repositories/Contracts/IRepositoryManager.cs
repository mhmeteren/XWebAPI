
namespace Repositories.Contracts
{
    public interface IRepositoryManager
    {
        IUsersRepository Users { get; }
        IFollowsRepository Follows { get; }
        IBlockedUsersRepository BlockedUsers { get; }

        Task SaveAsync();
    }
}
