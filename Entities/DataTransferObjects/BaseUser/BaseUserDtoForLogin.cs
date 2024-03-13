
using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects.BaseUser
{
    public record BaseUserDtoForLogin
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; init; }


        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; init; }

    }
}
