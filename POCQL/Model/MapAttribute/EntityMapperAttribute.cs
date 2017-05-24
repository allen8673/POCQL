using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model.MapAttribute
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EntityMapperAttribute : Attribute
    {
        public EntityMapperAttribute() { }

        /// <summary>
        /// Table對應設定
        /// </summary>
        /// <param name="table">
        /// ORM主要的Table；
        /// 如果ColumnMapperAttribute沒有設定Table就會預設帶此設定。
        /// </param>
        public EntityMapperAttribute(string table)
        {
            this.MainTable = table;
        }

        /// <summary>
        /// Table對應設定
        /// </summary>
        /// <param name="type">映射型態</param>
        public EntityMapperAttribute(MapType type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Table對應設定
        /// </summary>
        /// <param name="table">
        /// ORM主要的Table；
        /// 如果ColumnMapperAttribute沒有設定Table就會預設帶此設定。
        /// </param>
        /// <param name="type">映射型態</param>
        public EntityMapperAttribute(string table, MapType type)
            : this(table)
        {
            this.Type = type;
        }

        /// <summary>
        /// 主要映射的Table
        /// </summary>
        private string _MainTable;
        public string MainTable
        {
            get
            {
                return _MainTable ?? string.Empty;
            }
            private set { this._MainTable = value; }
        }

        /// <summary>
        /// 映射型態
        /// </summary>
        public MapType Type { get; private set; }
    }
}
