namespace Entities.Models
{
    public class TweetLikes
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public string TweetId { get; set; }
        public Tweets Tweets { get; set; }

        public string UserId { get; set; }
        public Users User { get; set; }
    }
}
