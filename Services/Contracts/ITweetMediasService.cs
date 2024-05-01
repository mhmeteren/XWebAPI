using Microsoft.AspNetCore.Http;

namespace Services.Contracts
{
    public interface ITweetMediasService
    {
        Task SaveMediasByTweet(string tweetId, List<IFormFile> medias);

        Task DeleteMediasByIds(string tweetId, List<string> mediaIds);
    }
}
