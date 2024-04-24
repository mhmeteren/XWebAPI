
namespace Entities.Models
{
    public class TweetMedias
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string path { get; set; }

        public string TweetId { get; set; }
        public Tweets Tweets { get; set; }
    }

}
