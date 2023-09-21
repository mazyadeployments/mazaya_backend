using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MMA.WebApi.Shared.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.FirstOrDefault().Description;
            }

            return value.ToString();
        }


        public static string GetDisplay(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DisplayAttribute[] attributes = fi.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.FirstOrDefault().Description;
            }

            return value.ToString();
        }
    }
}
