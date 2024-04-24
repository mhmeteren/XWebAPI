
namespace Entities.Exceptions.Tweets
{
    public class TweetsNotFoundException()
    : NotFoundException($"Tweet is not Found.")
    {
    }
}
