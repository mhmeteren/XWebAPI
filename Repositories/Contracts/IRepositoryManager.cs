
namespace Repositories.Contracts
{
    public interface IRepositoryManager
    {
        IFollowsRepository Follows { get; }
        IBlockedUsersRepository BlockedUsers { get; }

        Task SaveAsync();
    }
}
