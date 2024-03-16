
namespace Presentation.Validators
{
    public static class ValidatorHelpers
    {

        public static bool BirthdayDateRange(DateTime birthdayDate)
        {
            int yearCalculate = DateTime.Today.Year - birthdayDate.Date.Year;
            return (yearCalculate is > 10 and < 75);
        }

    }
}
