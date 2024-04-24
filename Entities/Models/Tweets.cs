
namespace Entities.Models
{
    public class Tweets
    {

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string? MainTweetID { get; set; }
        public string? Content { get; set; }
        public int LikeCount { get; set; }
        public int ReTweetCount { get; set; }
        public int CommentCount { get; set; }
        public  bool IsEditing { get; set; }
        public  bool IsDeleting { get; set; }
        public bool IsRetweet { get; set; }
        public int AllowedRepliers { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;



        public string CreaterUserId { get; set; }
        public Users CreaterUser { get; set; }

        public Tweets? MainTweet { get; set; }
        public ICollection<TweetMedias>? TweetMedias { get; set; }
        public ICollection<TweetLikes>? TweetLikes { get; set; }
    }

}
