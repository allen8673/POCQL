using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model.MapAttribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnMapperAttribute : Attribute
    {
        /// <summary>
        /// 欄位對應設定
        /// </summary>
        public ColumnMapperAttribute() { }

        /// <summary>
        /// 欄位對應設定
        /// </summary>
        /// <param name="column">
        /// 對應欄位名；
        /// 如果有指定資料來源，設定格式為{Column}:{DataSource}；
        /// 設定資料來源後，INSERT和UPDATE會自動忽略Property帶的值
        /// </param>
        public ColumnMapperAttribute(string column)
        {
            this.Column = column;
        }

        /// <summary>
        /// 欄位對應設定
        /// </summary>
        /// <param name="column">
        /// 對應欄位名；
        /// 如果有指定資料來源，設定格式為{Column}:{DataSource}；
        /// 設定資料來源後，INSERT和UPDATE會自動忽略Property帶的值
        /// </param>
        /// <param name="mapType">映射型態</param>
        public ColumnMapperAttribute(string column, ColumnType mapType)
            :this(column)
        {
            this.MapType = mapType;
        }

        /// <summary>
        /// 欄位對應設定
        /// </summary>
        /// <param name="column">
        /// 對應欄位名；
        /// 如果有指定資料來源，設定格式為{Column}:{DataSource}；
        /// 設定資料來源後，INSERT和UPDATE會自動忽略Property帶的值
        /// </param>
        /// <param name="table">指定Table；可以利用{#Table Parameter#}格式將Table參數化，Ex:{#DOC_TABLE#}即是將指定Table參數化為'DOC_TABLE'</param>
        public ColumnMapperAttribute(string column, string table)
            : this(column)
        {
            this.Table = table;
        }

        /// <summary>
        /// 欄位對應設定
        /// </summary>
        /// <param name="column">
        /// 對應欄位名；
        /// 如果有指定資料來源，設定格式為{Column}:{DataSource}；
        /// 設定資料來源後，INSERT和UPDATE會自動忽略Property帶的值
        /// </param>
        /// <param name="table">指定Table；可以利用{#Table Parameter#}格式將Table參數化，Ex:{#DOC_TABLE#}即是將指定Table參數化為'DOC_TABLE'</param>
        /// <param name="mapType">映射型態</param>
        public ColumnMapperAttribute(string column, string table, ColumnType mapType)
            : this(column, table)
        {
            this.MapType = mapType;
        }

        /// <summary>
        /// 欄位名
        /// 如果有指定資料來源，設定格式為{Column}:{DataSource}
        /// 注意: 如果設定資料來源，則在INSERT和UPDATE時會自動忽略Property帶的值
        ///       但是不影響SELECT
        /// </summary>
        public string Column { get; private set; }

        /// <summary>
        /// 欄位所處的Table名
        /// </summary>
        public string Table { get; private set; }

        /// <summary>
        /// 映射型態
        /// </summary>
        public ColumnType MapType { get; private set; }
    }
}
