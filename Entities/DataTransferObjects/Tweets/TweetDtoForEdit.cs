using Microsoft.AspNetCore.Http;

namespace Entities.DataTransferObjects.Tweets
{
    public record TweetDtoForEdit(
    string Id,
    string? Content,
    string Repliers,
    List<IFormFile>? Medias,
    List<string>? DeletedMediaIds)
    {

    }

}
