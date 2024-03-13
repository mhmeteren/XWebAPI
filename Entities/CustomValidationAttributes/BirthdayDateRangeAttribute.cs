

using System.ComponentModel.DataAnnotations;

namespace Entities.CustomValidationAttributes
{
    internal class BirthdayDateRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime selectedDate = Convert.ToDateTime(value);
            int yearCalculate = DateTime.Today.Year - selectedDate.Date.Year;
            if (yearCalculate < 10 || 75 < yearCalculate)
            {
                return new ValidationResult("Invalid birthday date.");
            }

            return ValidationResult.Success;
        }
    }

}
