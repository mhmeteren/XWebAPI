
namespace Entities.Models
{
    public class Follows
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public bool RequestStatus { get; set; }
        public string FollowerId { get; set; }
        public Users FollowerUser { get; set; }

        public string FollowingId { get; set; }
        public Users FollowingUser { get; set; }

    }
}
