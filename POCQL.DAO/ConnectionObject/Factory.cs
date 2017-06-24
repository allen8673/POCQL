using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.DAO.ConnectionObject
{
    public abstract class Factory
    {
        /// <summary>
        /// 產生連線物件
        /// </summary>
        /// <returns></returns>
        public IDbConnection CreateConnection()
        {
            return InitialConnectionSetting();
        }

        /// <summary>
        /// Db ConnectionString Builder
        /// </summary>
        protected abstract DbConnectionStringBuilder ConnectionStringBuilder { get; }

        /// <summary>
        /// Db ConnectionString Builder setting Hook
        /// </summary>
        protected virtual Action<DbConnectionStringBuilder> ConnectionStringHook { get { return new Action<DbConnectionStringBuilder>((b) => { }); } }

        /// <summary>
        /// Db Connection Object
        /// </summary>
        protected abstract IDbConnection ConnectionObject { get; }


        /// <summary>
        /// 初始化連線物件設定
        /// </summary>
        /// <returns></returns>
        protected IDbConnection InitialConnectionSetting()
        {
            DbConnectionStringBuilder stringBulder = ConnectionStringBuilder;
            IDbConnection connectionObject = ConnectionObject;

            var stringBuilderProps = stringBulder.GetType().GetProperties();
            PropertyInfo stringBuilderProp;

            //初始化ConnectionString Builder
            foreach (var factoryProp in this.GetType().GetProperties())
            {
                stringBuilderProp = stringBuilderProps.FirstOrDefault(i => i.Name == factoryProp.Name);
                if (stringBuilderProp == null) continue;

                stringBuilderProp.SetValue(stringBulder, factoryProp.GetValue(this));
            }

            ConnectionStringHook(stringBulder);

            connectionObject.ConnectionString = stringBulder.ConnectionString;

            return connectionObject;
        }
    }
}
