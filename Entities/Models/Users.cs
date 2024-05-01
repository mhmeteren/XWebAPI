namespace Entities.Models
{
    public class Users : BaseUser
    {
        public DateTime? Birthday { get; set; }
        public string? About { get; set; }
        public string? Location { get; set; }
        public bool IsPrivateAccount { get; set; }
        public bool IsVerifiedAccount { get; set; }

        public int FollowerCount { get; set; }
        public int FollowingCount { get; set; }



        public ICollection<Follows>? Followers { get; set; }
        public ICollection<Follows>? Following { get; set; }

        public ICollection<BlockedUsers>? BlockedUsers { get; set; }

        public ICollection<Tweets>? Tweets { get; set; }
        public ICollection<TweetLikes>? TweetLikes { get; set; }
    }
}
