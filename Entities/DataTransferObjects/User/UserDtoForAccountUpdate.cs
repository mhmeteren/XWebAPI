using Entities.CustomValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects.User
{
    public record UserDtoForAccountUpdate
    {
        [Required(ErrorMessage = "FullName is required.")]
        [MinLength(5, ErrorMessage = "FullName must consist of at least 5 characters.")]
        [MaxLength(100, ErrorMessage = "FullName must consist of at maximum 100 characters.")]
        public string? FullName { get; init; }


        [Required(ErrorMessage = "About is required.")]
        [MinLength(5, ErrorMessage = "About must consist of at least 5 characters.")]
        [MaxLength(200, ErrorMessage = "About must consist of at maximum 200 characters.")]
        public string? About { get; init; }


        [Required(ErrorMessage = "Location is required.")]
        [MinLength(5, ErrorMessage = "Location must consist of at least 5 characters.")]
        [MaxLength(50, ErrorMessage = "Location must consist of at maximum 50 characters.")]
        public string? Location { get; init; }


        [Required(ErrorMessage = "Gender is required.")]
        [MinLength(1, ErrorMessage = "Gender must consist of at least 5 characters.")]
        [MaxLength(10, ErrorMessage = "Gender must consist of at maximum 50 characters.")]
        [GenderValidation]
        public string? Gender { get; init; }
        


        [Required(ErrorMessage = "Birthday is required.")]
        [BirthdayDateRange]
        public DateTime Birthday { get; init; }
    }

}
