using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.SqlObject
{
    interface IMoveDataObject<TDerive> : IBaseObject<TDerive>
    {
        /// <summary>
        /// 設定移動資料的目的Table
        /// </summary>
        /// <param name="table">欲寫入資料的Table</param>
        /// <returns></returns>
        TDerive Into(string table);
    }
}
