using Entities.DataTransferObjects.User;

namespace Entities.DataTransferObjects.BlockedUsers
{
    public record BlockedUserDto
    {
        public UserDtoForBlocked? BlockedUser { get; init; }
    }
}
