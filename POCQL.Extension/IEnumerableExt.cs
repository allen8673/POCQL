using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Extension
{
    public static class IEnumerableExt
    {
        /// <summary>
        /// IEnumerable Insert method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas">IEnumerable Datas</param>
        /// <param name="index">Insert Index</param>
        /// <param name="data">Insert Data</param>
        /// <returns></returns>
        public static IEnumerable<T> ExtInsert<T>(this IEnumerable<T> datas, int index, T data)
        {
            List<T> result = datas.ToList();
            result.Insert(index, data);
            return result.Where(i => i != null);
        }
    }
}
