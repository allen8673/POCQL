using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Process.Helper
{
    /// <summary>
    /// 設定必定要取得的Property(對應的欄位)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMustAppend<T>
        where T : class
    {
        /// <summary>
        /// 設定必定要取得的欄位的Property
        /// </summary>
        /// <returns></returns>
        AppendProperty<T> MustAppendItem();
    }
}
