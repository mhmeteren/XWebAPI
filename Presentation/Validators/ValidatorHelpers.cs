
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



        public static bool ValidateNullableString(string value, int maxLength)
        {
            return value == null || (value.Length <= maxLength);
        }

        public static bool ValidateNullableList<T>(IEnumerable<T> list, int maxCount) where T : class
        {
            return list == null || list.Count() <= maxCount;
        }

        public static bool IsEnumValue<TEnum>(string value) where TEnum : struct, Enum
        {
            return Enum.TryParse(value, out TEnum enumValue) && Enum.IsDefined(typeof(TEnum), enumValue);
        }

    }
}
