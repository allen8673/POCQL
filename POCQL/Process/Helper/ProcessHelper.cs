using POCQL.Model.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Process.Helper
{
    internal static class ProcessHelper
    {
        /// <summary>
        /// 檢核物件是否符合有設定EntityMapperAttribute的規範
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj">全域物件(Domain Model)</param>
        /// <param name="func">檢核完成後所要執行的動作</param>
        /// <returns></returns>
        internal static TResult ObjectAuth<T, TResult>(this T obj, Func<EntityMapperAttribute, TResult> func)
            where T : class
        {
            EntityMapperAttribute attr = obj.GetType().GetCustomAttribute<EntityMapperAttribute>(true) as EntityMapperAttribute;
            if (attr == null || attr.Type != MapType.Column2Property)
                throw new Exception("指定的Domain物件並未給予'TableMapperAttribute'設定，或者映射型態並非為Column2Property");

            return func(attr);
        }

        /// <summary>
        /// 取得Lambda Expression中指定的Property Name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp">Lambda Expression</param>
        /// <returns></returns>
        public static string GetPropertyName<T>(this Expression<Func<T, object>> exp)
        {
            // *** 解析Lambda Expression，並取得內容資訊 ***
            MemberExpression member = exp.Body as MemberExpression;

            if (member == null)
            {
                UnaryExpression unary = exp.Body as UnaryExpression;
                member = unary.Operand as MemberExpression;
            }

            // *** 取得Property Name ***
            string propName = member.Member.Name;

            return propName;
        }
    }
}
