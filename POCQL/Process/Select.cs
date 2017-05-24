using POCQL.Model;
using POCQL.SqlObject;
using POCQL.Process.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POCQL.MSSQL;
using POCQL.ToolExt;
using Dao;

namespace POCQL
{
    public static class Select
    {
        private static ITransaction Trans { get { return DbConnectFactory.CreateConnection(""); } }

        /// <summary>
        /// Select Distinct ...
        /// </summary>
        /// <param name="mapToProp">
        ///     是否轉換結果的名字，如果要，最後會以{Column} AS {Property Name}輸出結果
        /// </param>
        /// <returns></returns>
        public static SelectObject Distinct(bool mapToProp = true)
        {
            SelectObject obj = new SelectObject(new ColumnSet[] { }, true, mapToProp);
            return obj;
        }

        /// <summary>
        /// Select Top ...
        /// </summary>
        /// <param name="top">Top Number</param>
        /// <param name="mapToProp">
        ///     是否轉換結果的名字，如果要，最後會以{Column} AS {Property Name}輸出結果
        /// </param>
        /// <returns></returns>
        public static SelectObject Top(int top, bool mapToProp = true)
        {
            SelectObject obj = new SelectObject(new ColumnSet[] { }, top, mapToProp);
            return obj;
        }

        /// <summary>
        /// 設定要查詢的欄位
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapToProp">
        ///     是否轉換結果的名字，如果要，最後會以{Column} AS {Property Name}輸出結果
        /// </param>
        /// <returns></returns>
        public static SelectObject Columns<T>(bool mapToProp = true)
            where T : class
        {
            SelectObject obj = new SelectObject(Utility.GetColumns<T>(mapToProp));
            return obj;
        }

        /// <summary>
        /// 設定要查詢的欄位
        /// </summary>
        /// <typeparam name="TDomain">Domain Model Type</typeparam>
        /// <typeparam name="TView">View Model Type</typeparam>
        /// <param name="mapToProp">
        ///     是否轉換結果的名字，如果要，最後會以{Column} AS {Property Name}輸出結果
        /// </param
        /// <returns></returns>
        public static SelectObject Columns<TDomain, TView>(bool mapToProp = true)
            where TDomain : class
        {
            SelectObject obj = new SelectObject(Utility.GetColumns<TDomain, TView>(mapToProp));
            return obj;
        }

        /// <summary>
        /// 依指定Match值設定要查詢的欄位
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matche">指定的Match值</param>
        /// <returns></returns>
        public static SelectObject MatcheColumns<T>(string matche)
            where T : class
        {
            SelectObject obj = new SelectObject(Utility.GetMatchColumns<T>(matche));
            return obj;
        }

        /// <summary>
        /// 設定要查詢的欄位
        /// </summary>
        /// <param name="columnSets">欄位資訊</param>
        /// <param name="ignoreDataSource">是否要忽略欄位設定的資料來源</param>
        /// <returns></returns>
        public static SelectObject Columns(IEnumerable<ColumnSet> columnSets, bool ignoreDataSource = true)
        {
            SelectObject obj = new SelectObject(columnSets, ignoreDataSource);
            return obj;
        }

        /// <summary>
        /// 設定要查詢的欄位
        /// </summary>
        /// <param name="columns">欲查詢欄位；若要給予Column別名，字串格式為{Column}:{別名}</param>
        /// <returns></returns>
        public static SelectObject Columns(params string[] columns)
        {
            List<ColumnSet> columnSets = new List<ColumnSet>();
            string[] items;
            foreach (string column in columns)
            {
                items = column.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                if (items.Count() < 2)
                    columnSets.Add(new ColumnSet(items[0].Trim()));
                else
                    columnSets.Add(new ColumnSet(items[0].Trim(), items[1]));
            }

            SelectObject obj = new SelectObject(columnSets);
            return obj;
        }

        #region Aggregate Methods
        /// <summary>
        /// SELECT COUNT(*) ...
        /// </summary>
        /// <returns></returns>
        public static SelectObject Count()
        {
            return Select.Columns("COUNT(*):[Total]");
        }

        /// <summary>
        /// SELECT COUNT(COLUMN) ...
        /// </summary>
        /// <param name="column">指定查詢數量的欄位；若要給予Column別名，字串格式為{Column}:{別名}</param>
        /// <returns></returns>
        public static SelectObject Count(string column)
        {
            return AggregateFunc(Aggregate.COUNT, column);
        }

        /// <summary>
        /// SELECT SUM(COLUMN) ...
        /// </summary>
        /// <param name="column">指定查詢總和的欄位；若要給予Column別名，字串格式為{Column}:{別名}</param>
        /// <returns></returns>
        public static SelectObject Sum(string column)
        {
            return AggregateFunc(Aggregate.SUM, column);
        }

        /// <summary>
        /// SELECT MAX(COLUMN) ...
        /// </summary>
        /// <param name="column">指定查詢最大的欄位；若要給予Column別名，字串格式為{Column}:{別名}</param>
        /// <returns></returns>
        public static SelectObject Max(string column)
        {
            return AggregateFunc(Aggregate.MAX, column);
        }

        /// <summary>
        /// SELECT MAX(COLUMN) ...
        /// </summary>
        /// <param name="max">指定查詢最大的欄位；若要給予Column別名，字串格式為{Column}:{別名}</param>
        /// <returns></returns>
        public static SelectObject Min(string column)
        {
            return AggregateFunc(Aggregate.MIN, column);
        }

        /// <summary>
        /// 處理所有Aggregate方法的共用方法
        /// </summary>
        /// <param name="aggregate">指定Aggregate類型</param>
        /// <param name="column">Aggregate欄位名稱，如果有別名就以':'指定別名，否則別名一律帶相對預設</param>
        /// <returns></returns>
        private static SelectObject AggregateFunc(Aggregate aggregate, string column)
        {
            string agtFunc = aggregate.GetDescription(),
                   mapTo = agtFunc;
            string[] columnName = column.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (columnName.Count() >= 2)
            {
                column = columnName[0];
                mapTo = columnName[1];
            }

            ColumnSet columSet = new ColumnSet($"{agtFunc}({column})", mapTo, false, false, true);
            return Select.Columns(new ColumnSet[] { columSet });
        }
        #endregion

        /// <summary>
        /// 執行查詢
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="trans">指定執行的ITransaction物件</param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(this SelectObject obj, ITransaction trans = null)
        {
            string sql = obj.ToString();
            Dictionary<string, object> finalParameters = obj.ConditionInfo.ToDictionary();

            trans = trans ?? Select.Trans;
            return trans.Query<T>(sql, finalParameters);
        }

        /// <summary>
        /// 執行查詢
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="parameters">查詢參數</param>
        /// <param name="trans">指定執行的ITransaction物件</param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(this SelectObject obj, object parameters, ITransaction trans = null)
        {
            string sql = obj.ToString();
            Dictionary<string, object> finalParameters = parameters.ToDictionary().MergeDictionary(obj.ConditionInfo.ToDictionary());

            trans = trans ?? Select.Trans;
            return trans.Query<T>(sql, finalParameters);
        }

        /// <summary>
        /// 執行查詢
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj"></param>
        /// <param name="parameters">查詢參數</param>
        /// <param name="map">物件對應關係</param>
        /// <param name="trans">指定執行的ITransaction物件</param>
        /// <returns></returns>
        public static IEnumerable<TResult> Query<T1, T2, TResult>(this SelectObject obj, object parameters, Func<T1, T2, TResult> map, ITransaction trans = null)
        {
            if (obj.SplitPoint.Count() != 1) throw new Exception("查詢型別數量與分割點數不符，請確定是否有明確設定型別之間的分割點");

            string sql = obj.ToString();
            Dictionary<string, object> finalParameters = parameters.ToDictionary().MergeDictionary(obj.ConditionInfo.ToDictionary());

            trans = trans ?? Select.Trans;
            return trans.Query<T1, T2, TResult>(sql, finalParameters, map, string.Join(",", obj.SplitPoint));
        }

        /// <summary>
        /// 執行查詢
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="trans">指定執行的ITransaction物件</param>
        /// <returns></returns>
        public static IEnumerable<IDictionary<string, object>> Query(this SelectObject obj, ITransaction trans = null)
        {
            trans = trans ?? Select.Trans;
            return trans.Query(obj.ToString(), obj.ConditionInfo.ToDictionary());
        }

        /// <summary>
        /// 執行查詢
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parameters">查詢參數</param>
        /// <param name="trans">指定執行的ITransaction物件</param>
        /// <returns></returns>
        public static IEnumerable<IDictionary<string, object>> Query(this SelectObject obj, object parameters, ITransaction trans = null)
        {
            string sql = obj.ToString();
            Dictionary<string, object> finalParameters = parameters.ToDictionary().MergeDictionary(obj.ConditionInfo.ToDictionary());

            trans = trans ?? Select.Trans;
            return trans.Query(sql, finalParameters);
        }

        /// <summary>
        /// 執行分頁查詢
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="trans">指定執行的ITransaction物件</param>
        /// <returns></returns>
        public static PagingResult<T> PagingQuery<T>(this SelectObject obj, ITransaction trans = null)
        {
            string sql = obj.Sql + Environment.NewLine + obj.CountSql;
            Dictionary<string, object> finalParameters = obj.ConditionInfo.ToDictionary();

            trans = trans ?? Select.Trans;
            var result = trans.QueryMultiple<T, int>(sql, finalParameters);

            return new PagingResult<T>(result.Item1.ToArray(), result.Item2.ToArray()[0]);
        }

        /// <summary>
        /// 執行分頁查詢
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj"></param>
        /// <param name="map">回傳型別對應Lambda</param>
        /// <param name="trans">指定執行的ITransaction物件</param>
        /// <returns></returns>
        public static TResult PagingQuery<TData, TResult>(this SelectObject obj, Func<PagingResult<TData>, TResult> map, ITransaction trans = null)
        {
            var result = obj.PagingQuery<TData>(trans);
            return map(result);

        }

        /// <summary>
        /// 執行分頁查詢
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="parameters">查詢參數</param>
        /// <param name="trans">指定執行的ITransaction物件</param>
        /// <returns></returns>
        public static PagingResult<T> PagingQuery<T>(this SelectObject obj, object parameters, ITransaction trans = null)
        {
            string sql = obj.Sql + Environment.NewLine + obj.CountSql;
            Dictionary<string, object> finalParameters = parameters.ToDictionary().MergeDictionary(obj.ConditionInfo.ToDictionary());

            trans = trans ?? Select.Trans;
            var result = trans.QueryMultiple<T, int>(sql, finalParameters);

            return new PagingResult<T>(result.Item1.ToArray(), result.Item2.ToArray()[0]);
        }

        /// <summary>
        /// 執行分頁查詢
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj"></param>
        /// <param name="parameters">查詢參數</param>
        /// <param name="map">回傳型別對應Lambda</param>
        /// <param name="trans">指定執行的ITransaction物件</param>
        /// <returns></returns>
        public static TResult PagingQuery<TData, TResult>(this SelectObject obj, object parameters, Func<PagingResult<TData>, TResult> map, ITransaction trans = null)
        {
            var result = obj.PagingQuery<TData>(parameters, trans);
            return map(result);
        }

        /// <summary>
        /// 查詢指定DB中的資料是否存在
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="trans">指定執行的ITransaction物件</param>
        /// <returns></returns>
        public static bool Exists(this SelectObject obj, ITransaction trans = null)
        {
            trans = trans ?? Select.Trans;
            return trans.Query<bool>(obj.ExistSql, obj.ConditionInfo.ToDictionary()).First();
        }

        /// <summary>
        /// 查詢指定DB中的資料是否存在
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="parameters">查詢參數</param>
        /// <param name="trans">指定執行的ITransaction物件</param>
        /// <returns></returns>
        public static bool Exists(this SelectObject obj, object parameters, ITransaction trans = null)
        {
            Dictionary<string, object> finalParameters = parameters.ToDictionary().MergeDictionary(obj.ConditionInfo.ToDictionary());

            trans = trans ?? Select.Trans;
            return trans.Query<bool>(obj.ExistSql, finalParameters).First();
        }

    }
}
