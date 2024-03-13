using Microsoft.AspNetCore.Identity;

namespace Entities.Models
{
    public class BaseUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public string? Gender { get; set; }



        public DateTime AccountCreateDate { get; set; } = DateTime.Now;
        public DateTime? AccountDisableDate { get; set; }

    }
}
