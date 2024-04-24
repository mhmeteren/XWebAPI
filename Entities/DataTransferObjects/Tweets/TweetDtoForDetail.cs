using Entities.DataTransferObjects.User;

namespace Entities.DataTransferObjects.Tweets
{
    public record TweetDtoForDetail
    {
        public string? Id { get; init; }
        public string? MainTweetID { get; set; }
        public string? Content { get; init; }
        public int? LikeCount { get; init; }
        public int? ReTweetCount { get; init; }
        public int? CommentCount { get; init; }
        public bool IsRetweet { get; set; }
        public bool? IsEditing { get; init; }
        public string? AllowedRepliers { get; init; }
        public DateTime? CreateDate { get; init; }
        public UserDtoForTweets? CreaterUser { get; init; }
        public List<string>? MediaPaths { get; init; }
        public TweetDtoForDetail? MainTweet { get; init; }
    }
}
