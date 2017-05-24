using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model.InternalAttribute
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    internal class NullDescriptionAttribute : Attribute
    {
        public NullDescriptionAttribute(string description)
        {
            this.Description = description;
        }

        public string Description { get; private set; }
    }

    internal static class NullDescriptionHelper
    {
        public static string GetNullDescription(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());
            if (fi == null) return string.Empty; //Add by Allen
            var attributes = fi.GetCustomAttribute(typeof(NullDescriptionAttribute), false) as NullDescriptionAttribute;
            return attributes != null ? attributes.Description : enumValue.ToString();
        }
    }
}
