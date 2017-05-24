using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model.Interface
{
    public interface ITableDetail
    {
        /// <summary>
        /// 欄位所處的Table名
        /// </summary>
        string TableName { get;}

        /// <summary>
        /// Table參數，外界設定格式為{#Table參數#}
        /// </summary>
        string TableParameter { get;}
    }
}
