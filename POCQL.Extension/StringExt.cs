using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Extension
{
    public static class StringExt
    {
        /// <summary>
        /// 即string.IsNullOrEmpty()
        /// </summary>
        /// <param name="value">目標字串</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string value)
        {
            value = value ?? "";
            return string.IsNullOrEmpty(value.Trim());
        }

        /// <summary>
        /// 在非空或NULL值字串中之間JOIN其他字串
        /// </summary>
        /// <param name="strs">目標字串陣列</param>
        /// <param name="value">Join值</param>
        /// <returns></returns>
        public static string StringJoin(this IEnumerable<string> strs, string value)
        {
            return string.Join(value, strs.Where(i => !i.IsNullOrEmpty()));
        }

        /// <summary>
        /// 如果指定字串是Null或是空字串，則回傳預設字串，否則就回傳指定字串本身
        /// </summary>
        /// <param name="value">目標字串</param>
        /// <param name="returnValu">如果是空值或Null，則回傳的值</param>
        /// <returns></returns>
        public static string IfNullOrEmpty(this string value, string returnVal)
        {
            return value.IsNullOrEmpty() ? returnVal : value;
        }

        /// <summary>
        /// String.Format
        /// </summary>
        /// <param name="str">目標字串</param>
        /// <param name="args">Format Value</param>
        /// <returns></returns>
        public static string StringFormat(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
    }
}
