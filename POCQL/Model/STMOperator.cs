using POCQL.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model
{
    /// <summary>
    /// SQLTool Model Operator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class STMOperator<T>
        where T : class
    {
        /// <summary>
        /// Object + Object
        /// </summary>
        /// <param name="obj1">Object 1</param>
        /// <param name="obj2">Object 2</param>
        /// <returns></returns>
        public static IEnumerable<T> operator +(STMOperator<T> obj1, STMOperator<T> obj2)
        {
            return TransType(new STMOperator<T>[] { obj1, obj2 }.Where(i => i != null));
        }

        /// <summary>
        /// Object + IEnumerable Object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="objs">IEnumerable Object</param>
        /// <returns></returns>
        public static IEnumerable<T> operator +(STMOperator<T> obj, IEnumerable<STMOperator<T>> objs)
        {
            if (objs == null) return TransType(new STMOperator<T>[] { obj }.Where(i => i != null));
            return TransType(objs.ExtInsert(0, obj));
        }

        /// <summary>
        /// IEnumerable Object + Object
        /// </summary>
        /// <param name="objs">IEnumerable Object</param>
        /// <param name="obj">Object</param>
        /// <returns></returns>
        public static IEnumerable<T> operator +(IEnumerable<STMOperator<T>> objs, STMOperator<T> obj)
        {
            if (objs == null) return TransType(new STMOperator<T>[] { obj }.Where(i => i != null));
            return TransType(objs.ExtInsert(objs.Count(), obj));
        }

        /// <summary>
        /// Object + Array Object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="objs">Array Object</param>
        /// <returns></returns>
        public static T[] operator +(STMOperator<T> obj, STMOperator<T>[] objs)
        {
            if (objs == null) return TransType(new STMOperator<T>[] { obj }.Where(i => i != null)).ToArray();
            return TransType(objs.ExtInsert(0, obj)).ToArray();
        }

        /// <summary>
        /// Array Object + Object
        /// </summary>
        /// <param name="objs">Array Object</param>
        /// <param name="obj">Object</param>
        /// <returns></returns>
        public static T[] operator +(STMOperator<T>[] objs, STMOperator<T> obj)
        {
            if (objs == null) return TransType(new STMOperator<T>[] { obj }.Where(i => i != null)).ToArray();
            return TransType(objs.ExtInsert(objs.Count(), obj)).ToArray();
        }

        /// <summary>
        ///  Object + List Object
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="objs">List Object</param>
        /// <returns></returns>
        public static List<T> operator +(STMOperator<T> obj, List<STMOperator<T>> objs)
        {
            if (objs == null) return TransType(new STMOperator<T>[] { obj }.Where(i => i != null)).ToList();
            return TransType(objs.ExtInsert(0, obj)).ToList();
        }

        /// <summary>
        /// List Object + Object
        /// </summary>
        /// <param name="objs">List Object</param>
        /// <param name="obj">Object</param>
        /// <returns></returns>
        public static List<T> operator +(List<STMOperator<T>> objs, STMOperator<T> obj)
        {
            if (objs == null) return TransType(new STMOperator<T>[] { obj }.Where(i => i != null)).ToList();
            return TransType(objs.ExtInsert(objs.Count, obj)).ToList();
        }

        /// <summary>
        /// 資料型別轉換
        /// </summary>
        /// <typeparam name="TD"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="datas">Data</param>
        /// <returns></returns>
        private static IEnumerable<T> TransType(IEnumerable<STMOperator<T>> datas)
        {
            return datas.Select(i => i as T);
        }
    }

}
