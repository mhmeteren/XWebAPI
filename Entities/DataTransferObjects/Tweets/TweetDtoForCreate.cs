using Microsoft.AspNetCore.Http;

namespace Entities.DataTransferObjects.Tweets
{
    public record TweetDtoForCreate(
        string? MainTweetID, 
        string? Content,
        string Repliers,
        bool IsRetweet, 
        List<IFormFile> Medias)
    {

    }
}
