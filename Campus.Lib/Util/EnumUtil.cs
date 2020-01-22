using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Campus.Lib.Util
{
    public static class EnumUtil
    {
        public static string PegarDescricaoEnum(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static string PegarDescricaoEnum<T>(T value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());


            DisplayNameAttribute[] displayattributes =
                (DisplayNameAttribute[])fi.GetCustomAttributes(
                    typeof(DisplayNameAttribute),
                    false);

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            

            if (displayattributes?.Length > 0 )
                return attributes[0].Description;
            else if (attributes?.Length > 0)
                return displayattributes[0].DisplayName;
            return value.ToString();
        }

        public static List<SelectListItem> EnumToSelectListItem<T>()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Generic argument type must be an EnumModel.");

            return Enum.GetValues(typeof(T)).Cast<T>().Select(v => new SelectListItem
            {
                Text = PegarDescricaoEnum(v),
                Value = (Convert.ToInt32(v)).ToString()
            }).ToList();
        }
    }
}
