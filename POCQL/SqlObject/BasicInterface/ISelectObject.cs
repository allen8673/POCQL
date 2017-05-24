using POCQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.SqlObject
{
    interface ISelectObject<TDerive> : IBaseObject<TDerive>, IReserved<TDerive>
        where TDerive : class
    {
        string Sql { get; }
        string CountSql { get; }
        string ExistSql { get; }

        /// <summary>
        /// 設定要查詢的欄位
        /// </summary>
        /// <param name="columns">要查詢的欄位，字串格式可以為{Column}或{Column}:{Map to Property}</param>
        /// <returns></returns>
        TDerive Columns(params string[] columns);

        /// <summary>
        /// 設定要查詢的欄位
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        TDerive Columns<T>() where T : class;

        /// <summary>
        /// 設定要查詢的欄位
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TView"></typeparam>
        /// <param name="domainProperties">
        ///     強制查詢的Domain Model Property；
        ///     如果有一定要查詢的Domain Model Property，但是擔心View Model沒有指定，可以強制指定。
        /// </param>
        /// <returns></returns>
        TDerive Columns<TDomain, TView>()
            where TDomain : class;

        /// <summary>
        /// 設定要查詢的欄位
        /// 依指定Match值設定要查詢的欄位
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matche"></param>
        /// <returns></returns>
        TDerive MatcheColumns<T>(string matche) where T : class;

        /// <summary>
        /// 設定子查詢
        /// </summary>
        /// <param name="select">子查詢SELECT物件</param>
        /// <param name="alias">子查詢結果別名</param>
        /// <returns></returns>
        TDerive SubQuery(BaseSelectObject<TDerive> select, string alias);

        /// <summary>
        /// 設定子查詢，並將其指派給指定型別的Property。
        /// 如果指定型別沒有該Property就會忽略不做
        /// </summary>
        /// <typeparam name="DType"></typeparam>
        /// <typeparam name="VType"></typeparam>
        /// <param name="select">子查詢SELECT物件</param>
        /// <param name="propertyMap">Property映射</param>
        /// <returns></returns>
        TDerive SubQuery<DType, VType>(BaseSelectObject<TDerive> select, Expression<Func<DType, object>> propertyMap);

        /// <summary>
        /// 設定子查詢，並將其指派給指定型別的Property。
        /// 如果指定型別沒有該Property就會忽略不做
        /// </summary>
        /// <typeparam name="DType"></typeparam>
        /// <typeparam name="VType"></typeparam>
        /// <param name="select">子查詢SQL</param>
        /// <param name="propertyMap">Property映射</param>
        /// <returns></returns>
        TDerive SubQuery<DType, VType>(string select, Expression<Func<DType, object>> propertyMap);


        /// <summary>
        /// 設定多型態的分界點
        /// </summary>
        /// <param name="splitPointName"></param>
        /// <returns></returns>
        TDerive SplitOn(string splitPointName = "");

        /// <summary>
        /// 如果有設定Table參數(格式為{#param#})，則將'{#param#}'轉成指定Table
        /// </summary>
        /// <param name="tableParameter">Table參數</param>
        /// <param name="currentTable">指定Table</param>
        /// <returns></returns>
        TDerive SetTable(string tableParameter, string currentTable);

        /// <summary>
        /// 設定查詢Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <returns></returns>
        TDerive From(string table);

        /// <summary>
        /// 設定查詢Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="tableAlias">欲查詢的Table別名</param>
        /// <returns></returns>
        TDerive From(string table, string tableAlias);

        /// <summary>
        /// 設定要INNER JOIN的Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="joinCondition">INNER JOIN的條件式</param>
        /// <returns></returns>
        TDerive InnerJoin(string table, string joinCondition);

        /// <summary>
        /// 設定要LEFT JOIN的Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="joinCondition">INNER JOIN的條件式</param>
        /// <returns></returns>
        TDerive LeftJoin(string table, string joinCondition);

        /// <summary>
        /// 設定要INNER JOIN的Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="tableAlias">欲查詢的Table別名</param>
        /// <param name="joinCondition">INNER JOIN的條件式</param>
        /// <returns></returns>
        TDerive InnerJoin(string table, string tableAlias, string joinCondition);

        /// <summary>
        /// 設定要LEFT JOIN的Table
        /// </summary>
        /// <param name="table">欲查詢的Table</param>
        /// <param name="tableAlias">欲查詢的Table別名</param>
        /// <param name="joinCondition">INNER JOIN的條件式</param>
        /// <returns></returns>
        TDerive LeftJoin(string table, string tableAlias, string joinCondition);

        /// <summary>
        /// 設定Union
        /// </summary>
        /// <param name="tableParameter">指定Table參數</param>
        /// <param name="unionTables">指定Union Table</param>
        /// <returns></returns>
        //TDerive Union(string tableParameter, params string[] unionTables);

    }
}
