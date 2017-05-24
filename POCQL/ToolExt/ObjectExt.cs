using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.ToolExt
{
    internal static class ObjectExt
    {
        /// <summary>
        /// 將物件轉換成Dictionary
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this object obj)
        {
            if (obj.GetType() == typeof(Dictionary<string, object>))
                return obj as Dictionary<string, object>;

            Dictionary<string, object> dict = new Dictionary<string, object>();

            foreach (var prop in obj.GetType().GetProperties())
            {
                dict.Add(prop.Name, prop.GetValue(obj));
            }
            return dict;
        }
    }
}
