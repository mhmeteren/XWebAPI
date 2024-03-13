
using Entities.Models;

namespace Repositories.Contracts
{
    public interface IUsersRepository : IRepositoryBase<Users>
    {

        Task<Users> GetUserByUsernameAsync(string Username, bool trackChanges);

    }
}
