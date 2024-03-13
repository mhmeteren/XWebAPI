namespace Entities.Models
{
    public class BlockedUsers
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string BlockerUserId { get; set; }
        public Users BlockerUser { get; set; }

        public string BlockedUserId { get; set; }
        public Users BlockedUser { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
