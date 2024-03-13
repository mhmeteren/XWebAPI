
using Entities.DataTransferObjects.User;

namespace Entities.DataTransferObjects.Follower
{
    public record FollowerDto
    {
        //public string Id { get; init; }
        public UserDtoForFollow? FollowerUser { get; init; }
    }

}
