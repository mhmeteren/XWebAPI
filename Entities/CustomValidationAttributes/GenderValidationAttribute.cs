

using System.ComponentModel.DataAnnotations;

namespace Entities.CustomValidationAttributes
{
    internal class GenderValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[] AllowedGenders = ["Female", "Male", "Other"];
            string? gender = value?.ToString();
            if (string.IsNullOrEmpty(gender) || !AllowedGenders.Contains(gender))
            {
                return new ValidationResult("Invalid gender value.");
            }

            return ValidationResult.Success;
        }
    }

}
