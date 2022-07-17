using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OMDb.Core.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return null;
            }
            FieldInfo[] fields = obj.GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    object[] attr = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (obj.ToString() == field.Name)
                        return attr.Length == 0 ? field.Name : ((DescriptionAttribute)attr[0]).Description;
                }
            }
            return null;
        }
    }
}
