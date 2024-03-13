using System.Globalization;

namespace Entities.UtilityClasses
{
    public static class Roles
    {

        public const string Admin = "Admin";
        public const string User = "User";

        public static string CustomToUpper(this string role)
        {
            return role.ToUpper(CultureInfo.InvariantCulture);
        }
    }
}
