

using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;

namespace Repositories.EFCore
{
    public class UsersRepository(RepositoryContext context) : RepositoryBase<Users>(context), IUsersRepository
    {
        public async Task<Users> GetUserByUsernameAsync(string Username, bool trackChanges) =>
            await FindByCondition(u => u.UserName.Equals(Username), trackChanges)
                .SingleOrDefaultAsync();

    }
}
