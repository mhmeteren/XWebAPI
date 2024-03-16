
namespace Presentation.Validators
{
    public static class ValidatorHelpers
    {
        private static readonly List<string> genderList = ["Female", "Male", "Other"];



        public static bool IsValidBirthdayDate(DateTime? birthdayDate)
        {
            if(!birthdayDate.HasValue)
                return true;

            int yearCalculate = DateTime.Today.Year - birthdayDate.Value.Year;
            return (yearCalculate is > 10 and < 75);
        }


        public static bool IsValidGender(string gender)
        {
            return genderList.Contains(gender);
        }

    }
}
