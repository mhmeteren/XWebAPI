
using Entities.DataTransferObjects.User;

namespace Entities.DataTransferObjects.Follower
{
    public record FollowingDto
    {
        //public string Id { get; init; }
        public UserDtoForFollow? FollowingUser { get; init; }
    }
}
