using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Process.Helper
{
    /// <summary>
    /// Nullable Property設定
    /// </summary>
    public class NullableList : List<string> { }

    /// <summary>
    /// Nullable Property設定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NullableList<T> : NullableList
        where T : class
    {
        public void Add(Expression<Func<T, object>> func)
        {
            base.Add(func.GetPropertyName());
        }
    }

}
