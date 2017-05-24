using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model.MapAttribute
{
    /// <summary>
    /// 設定為Primary Key的Property，在UPATE時不會異動資料
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute() { }
        public PrimaryKeyAttribute(bool autoIncrement)
        {
            this.AutoIncrement = autoIncrement;
        }

        /// <summary>
        /// 自動增長
        /// </summary>
        public bool AutoIncrement { get; private set; }
    }
}
