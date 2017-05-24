using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Process.Helper
{
    public class AppendProperty<T> : List<Expression<Func<T, object>>>
        where T : class
    {
        public IEnumerable<string> ParsePropertyName()
        {
            foreach (var item in this)
            {
                yield return item.GetPropertyName();
            }
        }
    }
}
