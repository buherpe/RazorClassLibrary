using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RazorClassLibrary
{
    public static class Extensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName();
        }

        //public static string GetDescription(this Enum enumValue)
        //{
        //    return enumValue.GetType()
        //        .GetMember(enumValue.ToString())
        //        .First()
        //        .GetCustomAttribute<DescriptionAttribute>()?
        //        .Description;
        //}

        public static object GetPropertyValue(object obj, string propertyName)
        {
            foreach (var prop in propertyName.Split('.').Select(s => obj?.GetType().GetProperty(s)))
            {
                obj = prop?.GetValue(obj, null);
            }

            return obj;
        }

        public static int ToInt(this Enum @enum)
        {
            return Convert.ToInt32(@enum);
        }
    }
}
