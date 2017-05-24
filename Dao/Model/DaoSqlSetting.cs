using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao.Model
{
    /// <summary>
    /// SQL暫存容器
    /// </summary>
    public class DaoSqlSetting
    {
        public DaoSqlSetting()
        {
            this.Parameters = new Dictionary<string, object>();
        }

        public DaoSqlSetting(string sql) :this()
        {
            this.Sql = sql;
        }

        public DaoSqlSetting(string sql, object parameters)
            : this(sql)
        {
            this.Parameters = parameters;
        }

        public string Sql { get; set; }
        public Object Parameters { get; set; }

        public static IEnumerable<DaoSqlSetting> operator +(IEnumerable<DaoSqlSetting> array, DaoSqlSetting item)
        {
            List<DaoSqlSetting> list = array.ToList();
            list.Add(item);
            return list;
        }

        public static IEnumerable<DaoSqlSetting> operator +(DaoSqlSetting item1, DaoSqlSetting item2)
        {
            return new DaoSqlSetting[]{item1, item2};
        }

        public static IEnumerable<DaoSqlSetting> operator +(DaoSqlSetting item, IEnumerable<DaoSqlSetting> array )
        {
            return new DaoSqlSetting[] { item }.Concat(array);
        }

    }
}
