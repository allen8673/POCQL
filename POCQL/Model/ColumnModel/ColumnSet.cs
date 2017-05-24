using POCQL.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using POCQL.Model.MapAttribute;
using POCQL.ToolExt;

namespace POCQL.Model
{
    /// <summary>
    /// 欄位資訊
    /// </summary>
    public class ColumnSet : STMOperator<ColumnSet>, ITableDetail
    {
        #region Constructor
        public ColumnSet(string column, string mapTo, string fromTable, object value, bool isPk, bool readOnly, bool aggregate)
            : this(column, mapTo, fromTable, isPk, readOnly, aggregate)
        {
            this.Value = value;
        }

        internal ColumnSet(string column)
        {
            this.ColumnName = column;
        }

        internal ColumnSet(string column, string mapTo)
            : this(column)
        {
            this.Property = mapTo;
        }

        internal ColumnSet(string column, bool isPk, bool readOnly, bool aggregate)
            : this(column)
        {
            this.IsPrimaryKey = isPk;
            this.ReadOnly = readOnly;
            this.Aggregate = aggregate;
        }

        internal ColumnSet(string column, string mapTo, bool isPk, bool readOnly, bool aggregate)
            : this(column, isPk, readOnly, aggregate)
        {
            this.Property = mapTo;
        }

        internal ColumnSet(string column, string mapTo, string fromTable, bool isPk, bool readOnly, bool aggregate)
            : this(column, mapTo, isPk, readOnly, aggregate)
        {
            fromTable = fromTable ?? string.Empty;

            if (Regex.IsMatch(fromTable, @"{#\w+#}"))
                this.TableParameter = fromTable.Replace("{#", "").Replace("#}", "");
            else
                this.TableName = fromTable;
        }

        internal ColumnSet(string column, string mapTo, string fromTable, bool isPk, bool readOnly, bool aggregate, ColumnType columnMapType)
            : this(column, mapTo, fromTable, isPk, readOnly, aggregate)
        {
            this.ColumnMapType = columnMapType;
        }

        internal ColumnSet(string column, string mapTo, string fromTable, object value, string sqlOpt, bool isPk, bool readOnly, bool aggregate, ColumnType columnMapType)
            : this(column, mapTo, fromTable, value, isPk, readOnly, aggregate)
        {
            this.SqlOperator = sqlOpt;
            this.ColumnMapType = columnMapType;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 欄位名
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// 欄位所處的Table名
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// 欄位全名 = {Table Name}.{Column Name}
        /// </summary>
        public string FullColumnName { get { return $"{this.TableName}.{this.ColumnName}"; } }

        /// <summary>
        /// Table參數，外界設定格式為{#Table參數#}
        /// </summary>
        public string TableParameter { get; private set; }

        /// <summary>
        /// Map欄位的Property名
        /// </summary>
        public string Property
        {
            get { return this._Property; }
            private set
            {
                this._Property = value != null ? value.Replace("[", string.Empty)
                                                      .Replace("]", string.Empty)
                                                      .Trim() :
                                                 value;
            }
        }
        private string _Property;

        /// <summary>
        /// Map欄位的Property Value
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// SQL運算，字串格式為: OptString + {#COLUMN#} + OptString；
        /// Ex1: MAX({#COLUMN#}); 
        /// EX2: CASE WHEN {#COLUMN#} IS NULL THEN ... ELSE... END
        /// </summary>
        public string SqlOperator { get; private set; }

        /// <summary>
        /// 指定欄位的資料來源&來源Table，通常this.Value會等於NULL
        /// </summary>
        public DataSourceSet DataSource { get; private set; }

        /// <summary>
        /// 是否為主Key
        /// </summary>
        public bool IsPrimaryKey { get; private set; }

        /// <summary>
        /// 是否為讀
        /// </summary>
        public bool ReadOnly { get; private set; }

        /// <summary>
        /// 欄位是否為Aggregate
        /// </summary>
        public bool Aggregate { get; set; }

        /// <summary>
        /// 如果Column有指定Table，
        /// 但是在Source Table裡面沒有的話，
        /// 就會被設定為NULL；
        /// 此時 IsNullColumn = true
        /// </summary>
        public bool IsNullColumn { get; set; }

        /// <summary>
        /// Column映射型態
        /// </summary>
        public ColumnType ColumnMapType { get; private set; }
        #endregion

        /// <summary>
        /// 依Table參數重新設置TableName
        /// </summary>
        /// <param name="tabelParamMaps">Table參數表</param>
        internal void ResetTable(Dictionary<string, string> tabelParamMaps)
        {
            if (this.TableParameter.IsNullOrEmpty()) return;

            // *** 如果再Table參數的Dic裡有該Key，則要進行轉換 ***
            string newName;
            if (tabelParamMaps.TryGetValue(this.TableParameter, out newName))
            {
                this.TableName = newName;
                this.TableParameter = string.Empty;

            }
        }

        /// <summary>
        /// 將ColumnSet轉成字串
        /// </summary>
        /// <param name="tableAlias">Table對應別名資訊</param>
        /// <param name="ignoreDataSource">是否忽略DataSource</param>
        /// <param name="mapProperty">是否Map至Property</param>
        /// <returns>預設匯出成[Table].[Column] [Property]</returns>
        internal string ToString(Dictionary<string, string> tableAlias, bool ignoreDataSource, bool mapProperty)
        {
            return mapProperty ?
                $"{this.ToColumnString(tableAlias, ignoreDataSource)} [{this.Property.IfNullOrEmpty(this.ColumnName)}]" :
                $"{this.ToColumnString(tableAlias, ignoreDataSource)}";
        }

        /// <summary>
        /// 將ColumnSet匯出成[Table].[Column]
        /// </summary>
        /// <param name="tableAlias">Table對應別名資訊</param>
        /// <param name="ignoreDataSource">是否忽略DataSource</param>
        /// <returns>[Table].[Column]</returns>
        private string ToColumnString(Dictionary<string, string> tableAlias, bool ignoreDataSource)
        {
            // *** 如果Column有指定Table，但是在Source Table裡面沒有的話，直接指定'SELECT NULL' ***
            if (!this.TableName.IsNullOrEmpty() && !tableAlias.ContainsKey(this.TableName))
            {
                this.IsNullColumn = true;
                return "NULL";
            }

            // *** 如果ColumnMapTyp是DataSource的話，直接回傳 ***
            if (this.ColumnMapType == ColumnType.DataSource)
            {
                return this.ColumnName;
            }

            // *** 取得Column對應的Table名稱 ***
            string table = this.TableDetailProcess(tableAlias),
                   column = (ignoreDataSource || this.DataSource == null) ?
                             new string[] { table, this.ColumnName }.StringJoin(".") :
                             this.DataSource.SourceValue;

            return Regex.Replace(this.SqlOperator.IfNullOrEmpty("{#COLUMN#}"), "{#COLUMN#}",
                                 column, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 解析欄位對應資料來源字串
        /// </summary>
        /// <param name="dataDescript">
        ///     欄位對應資料來源字串；
        ///     格式為: {Column}:{DataSource};
        ///     Ex: CRT_DT:GETDATE()
        /// </param>
        /// <param name="isPk">是否為PK</param>
        /// <param name="readOnly">是否唯讀</param>
        /// <param name="aggregate">是否為aggregate</param>
        /// <returns></returns>
        internal static ColumnSet ParseDataSource(string dataDescript, bool isPk, bool readOnly, bool aggregate)
        {
            string[] set = dataDescript.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (set.Count() < 2) throw new Exception("欄位與資料來源描述錯誤，請確保字串格式為Column:DataSource");

            ColumnSet columnSet = columnSet = new ColumnSet(set[0].Trim(), isPk, readOnly, aggregate);
            columnSet.DataSource = new DataSourceSet(set[1].Trim());

            return columnSet;
        }

        /// <summary>
        /// 解析欄位對應資料來源字串
        /// </summary>
        /// <param name="dataDescript">
        ///     欄位對應資料來源字串；
        ///     格式為: {Column}:{DataSource};
        ///     Ex: CRT_DT:GETDATE()
        /// </param>
        /// <param name="columnTable">目標資料Table</param>
        /// <param name="sourceTable">資料來源Table</param>
        /// <param name="isPk">是否為PK</param>
        /// <param name="readOnly">是否唯讀</param>
        /// <param name="aggregate">是否為aggregate</param>
        /// <returns></returns>
        internal static ColumnSet ParseDataSource(string dataDescript, string columnTable, string sourceTable, bool isPk, bool readOnly, bool aggregate)
        {
            string[] set = dataDescript.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            ColumnSet columnSet = new ColumnSet(set[0].Trim(), null, columnTable, isPk, readOnly, aggregate);
            if (set.Count() < 2)
            {
                columnSet.DataSource = new DataSourceSet(set[0].Trim(), sourceTable);
            }
            else
            {
                columnSet.DataSource = new DataSourceSet(set[1].Trim(), sourceTable);
            }
            return columnSet;
        }

        /// <summary>
        /// 解析欄位對應資料來源字串
        /// </summary>
        /// <param name="dataDescript">
        ///     欄位對應資料來源字串；
        ///     格式為: {Column}:{DataSource};
        ///     Ex: CRT_DT:GETDATE()
        /// </param>
        /// <param name="columnTable">目標資料Table</param>
        /// <param name="sourceTable">資料來源Table</param>
        /// <param name="mapTo">對應的Property</param>
        /// <param name="isPk">是否為PK</param>
        /// <param name="readOnly">是否唯讀</param>
        /// <param name="aggregate">是否為aggregate</param>
        /// <param name="columnMapType">Column映射型態</param>
        /// <returns></returns>
        internal static ColumnSet ParseDataSource(string dataDescript, string columnTable, string sourceTable, string mapTo, bool isPk, bool readOnly, bool aggregate, ColumnType columnMapType = 0)
        {
            string[] set = dataDescript.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            ColumnSet columnSet = new ColumnSet(set[0].Trim(), mapTo, columnTable, isPk, readOnly, aggregate, columnMapType);
            if (set.Count() < 2)
            {
                columnSet.DataSource = new DataSourceSet(set[0].Trim(), sourceTable);
            }
            else
            {
                columnSet.DataSource = new DataSourceSet(set[1].Trim(), sourceTable);
            }
            return columnSet;
        }


    }
}
