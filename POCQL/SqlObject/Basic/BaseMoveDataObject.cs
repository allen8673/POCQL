using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.SqlObject
{
    /// <summary>
    /// 移動資料SQL為: DELETE FROM {Source Table} OUTPUT DELETED.* INTO {Target Table} WHERE {Condition}，
    /// 本質上與DELETE非常像，
    /// 所以直接繼承 BaseDeleteObject，
    /// 並且補足需要的Property
    /// </summary>
    /// <typeparam name="TDerive">繼承類別型態</typeparam>
    public abstract class BaseMoveDataObject<TDerive> : BaseDeleteObject<TDerive>, IMoveDataObject<TDerive>
        where TDerive : class
    {
        /// <summary>
        /// 設定移動資料的目的Table
        /// </summary>
        /// <param name="table">欲寫入資料的Table</param>
        /// <returns></returns>
        public TDerive Into(string table)
        {
            this.Table.SetTable(table);
            return this as TDerive;
        }
    }
}
