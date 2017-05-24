using Dao.ProcessObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao
{
    public class DbConnectFactory
    {
        /// <summary>
        /// 根據 connectionId 建立相對應的 DataBase Connection Object
        /// 這個是 for  Login 或是無法使用 IoC 的情境 (CurrentIdentity 為 null)
        /// </summary>
        /// <param name="connection">加密的連線字串</param>
        /// <returns></returns>
        public static ITransaction CreateConnection(string connectionId)
        {
            return new MSSqlTool();

        }
    }
}
