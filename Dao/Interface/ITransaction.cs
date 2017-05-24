using Dao.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dao
{
    public interface ITransaction
    {
        /// <summary>
        /// 執行SQL指令
        /// </summary>
        /// <param name="settings">執行SQL乘載容器</param>
        /// <returns></returns>
        int Excute(IEnumerable<DaoSqlSetting> settings);

        /// <summary>
        /// 執行SQL指令
        /// </summary>
        /// <param name="settings">執行SQL乘載容器</param>
        /// <returns></returns>
        int Excute(DaoSqlSetting settings);
        
        /// <summary>
        /// SQL Procedure查詢
        /// </summary>
        /// <param name="procedureName">欲執行的Procedure</param>
        /// <param name="parameters">執行參數</param>
        /// <param name="getResults">執行後欲取得結果</param>
        /// <returns></returns>
        Dictionary<string, object> ExcuteProcedure(string procedure, IEnumerable<ProcedureParam> parameters, params string[] getResults);
        
        /// <summary>
        /// 執行查詢SQL
        /// </summary>
        /// <typeparam name="T">回傳型別</typeparam>
        /// <param name="sql">查詢的SQL Command</param>
        /// <param name="parameters">SQL參數</param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(string sql, object parameters);
        
        /// <summary>
        /// 執行查詢SQL
        /// </summary>
        /// <typeparam name="T1">指定查詢型別1</typeparam>
        /// <typeparam name="T2">指定查詢型別2</typeparam>
        /// <typeparam name="TResult">回傳型別</typeparam>
        /// <param name="sql">查詢的SQL Command</param>
        /// <param name="parameters">SQL參數</param>
        /// <param name="map">物件對應關係</param>
        /// <param name="splitOn">所屬T1與T2的欄位分割點</param>
        /// <returns></returns>
        IEnumerable<TResult> Query<T1, T2, TResult>(string sql, object parameters, Func<T1, T2, TResult> map, string splitOn);
        
        /// <summary>
        /// 執行查詢SQL
        /// </summary>
        /// <param name="sql">查詢的SQL Command</param>
        /// <param name="parameters">SQL參數</param>
        /// <returns></returns>
        IEnumerable<IDictionary<string, object>> Query(string sql, object parameters);
        
        /// <summary>
        /// 多重結果查詢
        /// </summary>
        /// <param name="sql">查詢的SQL Command</param>
        /// <param name="parameters">SQL參數</param>
        /// <returns></returns>
        dynamic[] QueryMultiple(string sql, object parameters);
        
        /// <summary>
        /// 多重結果查詢
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="sql">查詢的SQL Command</param>
        /// <param name="parameters">SQL參數</param>
        /// <returns></returns>
        Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(string sql, object parameters);
        
        /// <summary>
        /// 多重結果查詢
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="sql">查詢的SQL Command</param>
        /// <param name="parameters">SQL參數</param>
        /// <returns></returns>
        Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(string sql, object parameters);
        
        /// <summary>
        /// 檢核Table或Field是否存在
        /// </summary>
        /// <param name="tableName">資料表名稱</param>
        /// <param name="fieldName">欄位名稱</param>
        /// <returns></returns>
        bool CheckTableField(string tableName, string fieldName = "");
    }
}
