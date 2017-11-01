using POCQL.DAO.Model;
using POCQL.Extension;
using POCQL.Model;
using POCQL.Process.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.SqlObject
{
    public abstract class BaseSelectObject<TDerive> : BaseObject<TDerive>
        where TDerive : class
    {
        public BaseSelectObject()
        {
            this.SplitPoint = new string[] { };
            this.GroupByItems = new string[] { };
            this.OrderByColumn = new OrderBySetting[] { };
        }

        #region Properties

        private string _Condition;
        /// <summary>
        /// SQL條件式(不包含'WHERE'字串)
        /// </summary>
        protected override string Condition
        {
            get
            {
                if (this._Condition.IsNullOrEmpty())
                    this._Condition = this.ConditionInfo.ConvertToString(this.Table);

                return this._Condition;
            }
            set
            {
                this._Condition = value;
            }
        }

        /// <summary>
        /// 是否Select Distinct
        /// </summary>
        protected bool IsDistinct { get; set; }

        /// <summary>
        /// Select Top
        /// </summary>
        protected int TopCount { get; set; }

        /// <summary>
        /// Order By目標欄位名稱 & 排序方式
        /// </summary>
        protected OrderBySetting[] OrderByColumn { get; private set; }

        /// <summary>
        /// 分頁設定:指定頁數(從1開始)
        /// </summary>
        protected int Page { get; private set; }

        /// <summary>
        /// 分頁設定:每頁幾筆資料
        /// </summary>
        protected int PageRow { get; private set; }

        /// <summary>
        /// Group項目
        /// </summary>
        protected string[] GroupByItems { get; private set; }

        /// <summary>
        /// 多型態查詢時，各個型別的分界點
        /// </summary>
        internal string[] SplitPoint { get; private set; }

        /// <summary>
        /// 是否Lock Table
        /// </summary>
        protected bool LockTable { get; private set; }

        /// <summary>
        /// SQL
        /// </summary>
        public virtual string Sql { get; }

        /// <summary>
        /// COUNT SQL
        /// </summary>
        public virtual string CountSql { get; }

        /// <summary>
        /// EXISTS SQL
        /// </summary>
        public virtual string ExistSql { get; }

        #endregion

        /// <summary>
        /// Select Top ...
        /// </summary>
        /// <param name="top">Top Number</param>
        /// <returns></returns>
        public TDerive Top(int top)
        {
            this.TopCount = top;
            return this as TDerive;
        }

        /// <summary>
        /// 設定要查詢的欄位
        /// </summary>
        /// <param name="columns">要查詢的欄位，字串格式可以為{Column}或{Column}:{Map to Property}</param>
        /// <returns></returns>
        public TDerive Columns(params string[] columns)
        {
            List<ColumnSet> columnSets = new List<ColumnSet>();
            string[] items;
            foreach (string column in columns)
            {
                items = column.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                if (items.Count() < 2)
                    columnSets.Add(new ColumnSet(items[0]));
                else
                    columnSets.Add(new ColumnSet(items[0], items[1]));
            }

            this.ColumnInfos = this.ColumnInfos.Union(columnSets).ToArray();
            return this as TDerive;
        }

        /// <summary>
        /// 設定要查詢的欄位
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="binder">Column Binder</param>
        /// <returns></returns>
        public TDerive Columns<TDomain>(ColumnBinder<TDomain> binder)
        {
            this.ColumnInfos = this.ColumnInfos.Union(binder.ParseColumn()).ToArray();
            return this as TDerive;
        }

        /// <summary>
        /// 設定要查詢的欄位
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TView"></typeparam>
        /// <param name="binder">Column Binder</param>
        /// <returns></returns>
        public TDerive Columns<TDomain, TView>(ColumnBinder<TDomain, TView> binder)
        {
            this.ColumnInfos = this.ColumnInfos.Union(binder.ParseColumn()).ToArray();
            return this as TDerive;
        }

        /// <summary>
        /// 設定要查詢的欄位
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapToProp">
        ///     是否轉換結果的名字，如果要，最後會以{Column} AS {Property Name}輸出結果
        /// </param>
        /// <returns></returns>
        public TDerive Columns<T>(bool mapToProp = true)
            where T : class
        {
            this.ColumnInfos = this.ColumnInfos.Union(Utility.GetColumns<T>(mapToProp)).ToArray();
            return this as TDerive;
        }

        /// <summary>
        /// 設定要查詢的欄位
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TView"></typeparam>
        /// <param name="mapToProp">
        ///     是否轉換結果的名字，如果要，最後會以{Column} AS {Property Name}輸出結果
        /// </param>
        /// <returns></returns>
        public TDerive Columns<TDomain, TView>(bool mapToProp = true)
            where TDomain : class
        {
            this.ColumnInfos = this.ColumnInfos.Union(Utility.GetColumns<TDomain, TView>(mapToProp)).ToArray();
            return this as TDerive;
        }

        /// <summary>
        /// 設定要查詢的欄位
        /// 依指定Match值設定要查詢的欄位
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matche"></param>
        /// <returns></returns>
        public TDerive MatcheColumns<T>(string matche)
            where T : class
        {
            this.ColumnInfos = this.ColumnInfos.Union(Utility.GetMatchColumns<T>(matche)).ToArray();
            return this as TDerive;
        }

        #region Aggregate Methods
        /// <summary>
        /// SELECT COUNT(COLUMN) ...
        /// </summary>
        /// <param name="column">指定查詢數量的欄位；若要給予Column別名，字串格式為{Column}:{別名}</param>
        /// <returns></returns>
        public TDerive Count(string column)
        {
            return AggregateFunc(Aggregate.COUNT, column);
        }

        /// <summary>
        /// SELECT SUM(COLUMN) ...
        /// </summary>
        /// <param name="column">指定查詢總和的欄位；若要給予Column別名，字串格式為{Column}:{別名}</param>
        /// <returns></returns>
        public TDerive Sum(string column)
        {
            return AggregateFunc(Aggregate.SUM, column);
        }

        /// <summary>
        /// SELECT MAX(COLUMN) ...
        /// </summary>
        /// <param name="column">指定查詢最大的欄位；若要給予Column別名，字串格式為{Column}:{別名}</param>
        /// <returns></returns>
        public TDerive Max(string column)
        {
            return AggregateFunc(Aggregate.MAX, column);
        }

        /// <summary>
        /// SELECT MAX(COLUMN) ...
        /// </summary>
        /// <param name="max">指定查詢最大的欄位；若要給予Column別名，字串格式為{Column}:{別名}</param>
        /// <returns></returns>
        public TDerive Min(string column)
        {
            return AggregateFunc(Aggregate.MIN, column);
        }

        /// <summary>
        /// 處理所有Aggregate方法的共用方法
        /// </summary>
        /// <param name="aggregate">指定Aggregate類型</param>
        /// <param name="column">Aggregate欄位名稱，如果有別名就以':'指定別名，否則別名一律帶相對預設</param>
        /// <returns></returns>
        private TDerive AggregateFunc(Aggregate aggregate, string column)
        {
            string agtFunc = aggregate.GetDescription(),
                   mapTo = $"{agtFunc}_{column}";
            string[] columnName = column.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (columnName.Count() >= 2)
            {
                column = columnName[0];
                mapTo = columnName[1];
            }

            this.ColumnInfos += new ColumnSet($"{agtFunc}({column})", mapTo, false, false, true);

            return this as TDerive;
        }
        #endregion

        /// <summary>
        /// 設定子查詢
        /// </summary>
        /// <param name="select">子查詢SELECT物件</param>
        /// <param name="alias">子查詢結果別名</param>
        /// <returns></returns>
        public TDerive SubQuery(BaseSelectObject<TDerive> select, string alias)
        {
            // *** 整合主查詢和子查詢的Conditions ***
            this.ConditionInfo = this.ConditionInfo.Union(select.ConditionInfo).ToArray();

            // *** 將子集合新增至查詢欄位資訊中 ***
            this.ColumnInfos = this.ColumnInfos + new ColumnSet($"({select})", alias, false, false, false);

            return this as TDerive;
        }

        /// <summary>
        /// 設定子查詢，並將其指派給指定型別的Property。
        /// 如果指定型別沒有該Property就會忽略不做
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="select">子查詢SELECT物件</param>
        /// <param name="propertyMap">Property映射</param>
        /// <returns></returns>
        public TDerive SubQuery<T>(BaseSelectObject<TDerive> select, Expression<Func<T, object>> propertyMap)
        {
            // *** 整合主查詢和子查詢的Conditions ***
            this.ConditionInfo = this.ConditionInfo.Union(select.ConditionInfo).ToArray();

            string propName = propertyMap.GetPropertyName();

            // *** 將子集合新增至查詢欄位資訊中 ***
            this.ColumnInfos = this.ColumnInfos + new ColumnSet($"({select})", propName, false, false, false);
            return this as TDerive;
        }

        /// <summary>
        /// 設定子查詢，並將其指派給指定型別的Property。
        /// 如果指定型別沒有該Property就會忽略不做
        /// </summary>
        /// <typeparam name="DType">Domain型別</typeparam>
        /// <typeparam name="VType">輸出Value型別</typeparam>
        /// <param name="select">子查詢SELECT物件</param>
        /// <param name="propertyMap">Property映射</param>
        /// <returns></returns>
        public TDerive SubQuery<DType, VType>(BaseSelectObject<TDerive> select, Expression<Func<DType, object>> propertyMap)
        {

            // *** 整合主查詢和子查詢的Conditions ***
            this.ConditionInfo = this.ConditionInfo.Union(select.ConditionInfo).ToArray();

            string propName = propertyMap.GetPropertyName();
            // *** 檢核View Model是否有該Property Name ***
            if (typeof(VType).GetProperties().Any(i => i.Name == propName))
            {
                // *** 將子集合新增至查詢欄位資訊中 ***
                this.ColumnInfos = this.ColumnInfos + new ColumnSet($"({select})", propName, false, false, false);
            }

            return this as TDerive;
        }

        /// <summary>
        /// 設定子查詢，並將其指派給指定型別的Property。
        /// 如果指定型別沒有該Property就會忽略不做
        /// </summary>
        /// <typeparam name="DType">Domain型別</typeparam>
        /// <typeparam name="VType">輸出Value型別</typeparam>
        /// <param name="select">子查詢SQL</param>
        /// <param name="propertyMap">Property映射</param>
        /// <returns></returns>
        public TDerive SubQuery<DType, VType>(string select, Expression<Func<DType, object>> propertyMap)
        {
            // *** 取得Domain Property Name ***
            string propName = propertyMap.GetPropertyName();

            // *** 檢核View Model是否有該Property Name ***
            if (typeof(VType).GetProperties().Any(i => i.Name == propName))
            {
                // *** 將子集合新增至查詢欄位資訊中 ***
                this.ColumnInfos = this.ColumnInfos + new ColumnSet($"({select})", propName, false, false, false);
            }
            return this as TDerive;
        }


        /// <summary>
        /// 設定多型態的分界點
        /// </summary>
        /// <param name="splitPointName"></param>
        /// <returns></returns>
        public TDerive SplitOn(string splitPointName = "")
        {
            splitPointName = splitPointName.IsNullOrEmpty() ?
                                $"Split_{this.SplitPoint.Count()}" :
                                splitPointName;

            this.SplitPoint = this.SplitPoint.Append(splitPointName);
            this.ColumnInfos = this.ColumnInfos.Append(new ColumnSet("'Split'", splitPointName, false, false, false));

            return this as TDerive;
        }

        /// <summary>
        /// 如果有設定Table參數(格式為{#param#})，則將'{#param#}'轉成指定Table
        /// </summary>
        /// <param name="tableParameter">Table參數</param>
        /// <param name="currentTable">指定Table</param>
        /// <returns></returns>
        public TDerive SetTable(string tableParameter, string currentTable)
        {
            this.TabelParameterMaps = new Dictionary<string, string>(this.TabelParameterMaps);

            if (this.TabelParameterMaps.ContainsKey(tableParameter))
                this.TabelParameterMaps[tableParameter] = currentTable;
            else
                this.TabelParameterMaps.Add(tableParameter, currentTable);

            return this as TDerive;
        }

        /// <summary>
        /// 設定查詢Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <returns></returns>
        public TDerive From(string table)
        {
            this.Table = new TableSet(table, null);
            return this as TDerive;
        }

        /// <summary>
        /// 設定查詢Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="tableAlias">欲查詢的Table別名</param>
        /// <returns></returns>
        public TDerive From(string table, string tableAlias)
        {
            this.Table = new TableSet(table, tableAlias);
            return this as TDerive;
        }

        /// <summary>
        /// 設定查詢Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="lockTable">是否Lock Table，預設為WITH (NOLOCK)</param>
        /// <returns></returns>
        public TDerive From(string table, bool lockTable)
        {
            this.LockTable = lockTable;
            return this.From(table);
        }

        /// <summary>
        /// 設定查詢Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="tableAlias">欲查詢的Table別名</param>
        /// <param name="lockTable">是否Lock Table，預設為WITH (NOLOCK)</param>
        /// <returns></returns>
        public TDerive From(string table, string tableAlias, bool lockTable)
        {
            this.LockTable = lockTable;
            return this.From(table, tableAlias);
        }

        /// <summary>
        /// 設定要INNER JOIN的Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="joinCondition">INNER JOIN的條件式</param>
        /// <returns></returns>
        public TDerive InnerJoin(string table, string joinCondition)
        {
            return InnerJoin(table, null, joinCondition);
        }

        /// <summary>
        /// 設定要LEFT JOIN的Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="joinCondition">LEFT JOIN的條件式</param>
        /// <returns></returns>
        public TDerive LeftJoin(string table, string joinCondition)
        {
            return LeftJoin(table, null, joinCondition);
        }

        /// <summary>
        /// 設定要INNER JOIN的Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="tableAlias">欲查詢的Table別名</param>
        /// <param name="joinCondition">INNER JOIN的條件式</param>
        /// <returns></returns>
        public TDerive InnerJoin(string table, string tableAlias, string joinCondition)
        {
            return this.InnerJoin(table, tableAlias, false, joinCondition);
        }

        /// <summary>
        /// 設定要LEFT JOIN的Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="tableAlias">欲查詢的Table別名</param>
        /// <param name="joinCondition">LEFT JOIN的條件式</param>
        /// <returns></returns>
        public TDerive LeftJoin(string table, string tableAlias, string joinCondition)
        {
            return this.LeftJoin(table, tableAlias, false, joinCondition);
        }

        /// <summary>
        /// 設定要INNER JOIN的Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="tableAlias">欲查詢的Table別名</param>
        /// <param name="lockScrTable">Join Table是否要Lock</param>
        /// <param name="joinCondition">INNER JOIN的條件式</param>
        /// <returns></returns>
        public TDerive InnerJoin(string table, string tableAlias, bool lockScrTable, string joinCondition)
        {
            this.Table.SetSourceTable(table, tableAlias, lockScrTable, JoinType.InnerJoin, joinCondition);
            return this as TDerive;
        }

        /// <summary>
        /// 設定要LEFT JOIN的Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="tableAlias">欲查詢的Table別名</param>
        /// <param name="lockScrTable">Join Table是否要Lock</param>
        /// <param name="joinCondition">LEFT JOIN的條件式</param>
        /// <returns></returns>
        public TDerive LeftJoin(string table, string tableAlias, bool lockScrTable, string joinCondition)
        {
            this.Table.SetSourceTable(table, tableAlias, lockScrTable, JoinType.LeftJoin, joinCondition);
            return this as TDerive;
        }

        /// <summary>
        /// 設定ORDER BY
        /// </summary>
        /// <param name="orderBy">要Order By的欄位或Property</param>
        /// <param name="defaultOrder">如果orderBy為空時，預設排序</param>
        /// <param name="sort">排序方式</param>
        /// <returns></returns>
        private TDerive OrderBy(string orderBy, string defaultOrder, SortKind sort = SortKind.ASC)
        {
            // *** 從既有的ColumnInfos中找到相同Property Name或Column Name的ColumnSet ***
            // *** 如果沒有找到對應的ColumnSet就直接帶預設Order ***
            //ColumnSet column = this.ColumnInfos
            //                       .Where(i => (i.Property == orderBy || i.ColumnName == orderBy))
            //                       .FirstOrDefault();

            //string columnName = column != null ? column.ColumnName : orderBy.OdExt_IfNullOrEmpty(defaultOrder);

            //this.OrderByColumn = this.OrderByColumn + new OrderBySetting(columnName, sort);

            this.OrderByColumn = this.OrderByColumn + new OrderBySetting(orderBy.IfNullOrEmpty(defaultOrder), sort);
            return this as TDerive;
        }


        /// <summary>
        /// 設定ORDER BY
        /// </summary>
        /// <param name="orderBy">要Order By的欄位或Property</param>
        /// <param name="defaultOrder">如果orderBy為空時，預設排序</param>
        /// <param name="sort">排序方式:ASC,DESC</param>
        /// <returns></returns>
        public TDerive OrderBy(string orderBy, string defaultOrder, string sort)
        {
            return this.OrderBy(orderBy, defaultOrder, sort.ToEnum<SortKind>());
        }

        /// <summary>
        /// 設定ORDER BY
        /// </summary>
        /// <param name="orderBy">要Order By的欄位或Property</param>
        /// <param name="sort">排序方式:ASC,DESC</param>
        /// <returns></returns>
        public TDerive OrderBy(string orderBy, string sort)
        {
            return this.OrderBy(orderBy, orderBy, sort);
        }

        /// <summary>
        /// 設定ORDER BY
        /// </summary>
        /// <param name="orderBy">要Order By的欄位或Property</param>
        /// <returns></returns>
        public TDerive OrderBy(string orderBy)
        {
            return this.OrderBy(orderBy, orderBy, "ASC");
        }

        /// <summary>
        /// 設定ORDER BY
        /// </summary>
        /// <param name="orderBy">要Order By的欄位或Property, 以及排序方式</param>
        /// <returns></returns>
        public TDerive OrderBy(OrderSet orderBy)
        {
            foreach (var item in orderBy)
                this.OrderBy(item.Key, item.Value);

            return this as TDerive;
        }

        /// <summary>
        /// 設定ORDER BY
        /// </summary>
        /// <param name="orderBy">要Order By的欄位或Property</param>
        /// <param name="sort">排序方式</param>
        /// <returns></returns>
        protected TDerive OrderBy(string orderBy, SortKind sort)
        {
            return this.OrderBy(orderBy, orderBy, sort);
        }

        /// <summary>
        /// 設定GROUP BY
        /// </summary>
        /// <param name="groupBy">
        /// 要Group By的欄位或Property；
        /// 注意: 如果Domain Model有掛AggregationAttribute的話，GroupBy功能需要給Model的Property Name
        /// </param>
        /// <returns></returns>
        public TDerive GroupBy(params string[] groupBy)
        {
            this.GroupByItems = groupBy;
            return this as TDerive;
        }


        /// <summary>
        /// 設定分頁
        /// </summary>
        /// <param name="pageRow">每頁資料量</param>
        /// <param name="page">第幾頁</param>
        /// <returns></returns>
        public TDerive Paging(int pageRow, int page)
        {
            this.PageRow = pageRow;
            this.Page = page;
            return this as TDerive;
        }

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <returns></returns>
        public override SqlSet Output()
        {
            string sql = this.ToString();
            return new SqlSet(sql, this.ConditionInfo.ToDictionary());
        }

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <param name="otherParam">其他指定參數</param>
        /// <returns></returns>
        public override SqlSet Output(object otherParam)
        {
            string sql = this.ToString();
            object parameters = Utility.OutputParameters(this.ConditionInfo.ToDictionary(), otherParam);
            return new SqlSet(sql, parameters);
        }

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <param name="otherParams">其他指定參數</param>
        /// <returns></returns>
        public override IEnumerable<SqlSet> Output(IEnumerable<object> otherParams)
        {
            throw new NotImplementedException("基於考量，目前不在SELECT提供此項功能");

            //List<SqlSet> sqlSets = new List<SqlSet>();
            //string sql = this.ToString();
            //object parameters;

            //foreach (object otherParam in otherParams) 
            //{
            //    parameters = Utility.OutputParameters(this.TemplateValue, otherParam);
            //    sqlSets.Add(new SqlSet(sql, parameters));
            //}

            //return sqlSets;
        }

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <typeparam name="T">指定輸出型別</typeparam>
        /// <returns></returns>
        public override T Output<T>()
        {
            return this.Output().ToType<T>();
        }

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="otherParam">其他指定參數</param>
        /// <returns></returns>
        public override T Output<T>(object otherParam)
        {
            return this.Output(otherParam).ToType<T>();
        }

        /// <summary>
        /// 輸出SQL和SQL需要的參數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="otherParams">其他指定參數</param>
        /// <returns></returns>
        public override IEnumerable<T> Output<T>(IEnumerable<object> otherParams)
        {
            throw new NotImplementedException("基於考量，目前不在SELECT提供此項功能");
        }


    }
}
