using Entities.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects.User
{
    public record UserDtoForRegister
    {
        [Required(ErrorMessage = "FullName is required.")]
        [MinLength(5, ErrorMessage = "FullName must consist of at least 5 characters.")]
        [MaxLength(100, ErrorMessage = "FullName must consist of at maximum 100 characters.")]
        public string? FullName { get; init; }


        [Required(ErrorMessage = "UserName is required.")]
        [MinLength(5, ErrorMessage = "UserName must consist of at least 5 characters.")]
        [MaxLength(100, ErrorMessage = "UserName must consist of at maximum 100 characters.")]
        //Custom filter
        public string? UserName { get; init; }


        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; init; }


        [Required(ErrorMessage = "Birthday is required.")]
        [BirthdayDateRange]
        public DateTime Birthday { get; init; }


        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; init; }
    }

}
