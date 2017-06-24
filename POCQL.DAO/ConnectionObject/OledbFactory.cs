using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.DAO.ConnectionObject
{
    /// <summary>
    /// 【Object Linking And Embedding Database(OLEDB)】Object Factory
    /// </summary>
    public class OledbFactory : Factory
    {
        public string Provider { get; set; }
        public string DataSource { get; set; }
        public string ExtendedProperties { get; set; }

        //Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\BAR.XLS;Extended Properties=""Excel 8.0;HDR=Yes;
        protected override IDbConnection ConnectionObject { get { return new OleDbConnection(); } }

        protected override Action<DbConnectionStringBuilder> ConnectionStringHook
        {
            get
            {
                return new Action<DbConnectionStringBuilder>((b) => { ((OleDbConnectionStringBuilder)b).Add("Extended Properties", ExtendedProperties); });
            }
        }

        protected override DbConnectionStringBuilder ConnectionStringBuilder
        {
            get { return new OleDbConnectionStringBuilder(); }
        }

    }
}
