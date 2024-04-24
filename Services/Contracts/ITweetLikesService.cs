
using Entities.Models;

namespace Services.Contracts
{
    public interface ITweetLikesService
    {

        Task CreateLike(string tweetId, string loggedInUserId);
        Task DeleteLike(string tweetId, string loggedInUserId);
    }
}
