using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Extension
{
    public  static class EnumExt
    {
        /// <summary>
        /// 如果Enum有加上[Description] Attribute，就可以使用此方法讀取 Description 內容
        /// </summary>
        /// <returns>回傳「Description 內容」或「列舉子的名稱」</returns>
        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());
            if (fi == null) return ""; //Add by Allen
            var attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();
        }
    }
}
