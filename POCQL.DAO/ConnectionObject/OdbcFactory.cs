using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.DAO.ConnectionObject
{
    /// <summary>
    /// 【Open Database Connectivity(odbc)】Object Factory
    /// </summary>
    public class OdbcFactory : Factory
    {
        /// <summary>
        /// 資料伺服器
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// 資料庫名稱
        /// </summary>
        public string InitialCatalog { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool PersistSecurityInfo { get; set; }

        /// <summary>
        /// 登入帳號
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 登入密碼
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// SqlConnectionStringBuilder
        /// </summary>
        protected override DbConnectionStringBuilder ConnectionStringBuilder
        {
            get
            {
                return new SqlConnectionStringBuilder();
            }
        }

        /// <summary>
        /// SqlConnection Object
        /// </summary>
        protected override IDbConnection ConnectionObject
        {
            get
            {
                return new SqlConnection();
            }
        }
    }
}
