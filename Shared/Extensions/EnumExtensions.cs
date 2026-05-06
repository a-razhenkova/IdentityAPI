using System.ComponentModel;
using System.Reflection;

namespace Shared
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            string description = value.ToString();

            FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo is not null)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes is not null && attributes.Length > 0)
                    description = attributes[0].Description;
            }

            return description;
        }
    }
}