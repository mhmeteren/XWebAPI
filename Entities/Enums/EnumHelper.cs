using System.ComponentModel;

namespace Entities.Enums
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum value)
        {
            var attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;

            return attribute?.Description ?? value.ToString();
        }
    }
}
