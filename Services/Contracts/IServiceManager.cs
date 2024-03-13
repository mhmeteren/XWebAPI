

namespace Services.Contracts
{
    public interface IServiceManager
    {
        IUserService UserService { get; }
        IAuthenticationService AuthenticationService { get; }
        IFollowsService FollowsService { get; }
        IBlockedUsersService BlockedUsersService { get; }
    }
}
