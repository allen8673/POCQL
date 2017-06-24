using Dapper;
using POCQL.DAO.ConnectionObject;
using POCQL.DAO.Model;
using POCQL.Extension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.DAO
{
    public class DataAccess
    {
        private Factory _ConnectionFac;
        private IDbConnection _DbCon;
        private DateTime _DaoTime;



        public DataAccess(Factory factory)
        {
            _ConnectionFac = factory;
        }

        /// <summary>
        /// 執行查詢SQL
        /// </summary>
        /// <typeparam name="T">回傳型別</typeparam>
        /// <param name="sql">查詢的SQL Command</param>
        /// <param name="parameters">SQL參數</param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, object parameters)
        {

            using (_DbCon = _ConnectionFac.CreateConnection())
            {
                return _DbCon.Query<T>(sql, parameters);
            }
        }

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
        public IEnumerable<TResult> Query<T1, T2, TResult>(string sql, object parameters, Func<T1, T2, TResult> map, string splitOn)
        {
            using (_DbCon = _ConnectionFac.CreateConnection())
            {
                return _DbCon.Query<T1, T2, TResult>(sql, map, parameters, splitOn: splitOn);
            }
        }

        /// <summary>
        /// 執行查詢SQL
        /// </summary>
        /// <param name="sql">查詢的SQL Command</param>
        /// <param name="parameters">SQL參數</param>
        /// <returns></returns>
        public IEnumerable<IDictionary<string, object>> Query(string sql, object parameters)
        {
            using (_DbCon = _ConnectionFac.CreateConnection())
            {
                return (IEnumerable<IDictionary<string, object>>)_DbCon.Query(sql, parameters);
            }
        }


        /// <summary>
        /// 多重結果查詢
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="sql">查詢的SQL Command</param>
        /// <param name="parameters">SQL參數</param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>> QueryMultiple<T1, T2>(string sql, object parameters)
        {
            using (_DbCon = _ConnectionFac.CreateConnection())
            {
                var result = _DbCon.QueryMultiple(sql, parameters);
                IEnumerable<T1> t1Obj = result.Read<T1>();
                IEnumerable<T2> t2Obj = result.Read<T2>();
                return Tuple.Create(t1Obj, t2Obj);
            }
        }

        /// <summary>
        /// 多重結果查詢
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="sql">查詢的SQL Command</param>
        /// <param name="parameters">SQL參數</param>
        /// <returns></returns>
        public Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> QueryMultiple<T1, T2, T3>(string sql, object parameters)
        {
            using (_DbCon = _ConnectionFac.CreateConnection())
            {
                var result = _DbCon.QueryMultiple(sql, parameters);
                IEnumerable<T1> t1Obj = result.Read<T1>();
                IEnumerable<T2> t2Obj = result.Read<T2>();
                IEnumerable<T3> t3Obj = result.Read<T3>();
                return Tuple.Create(t1Obj, t2Obj, t3Obj);
            }
        }

        /// <summary>
        /// 多重結果查詢
        /// </summary>
        /// <param name="sql">查詢的SQL Command</param>
        /// <param name="parameters">SQL參數</param>
        /// <returns></returns>
        public dynamic[] QueryMultiple(string sql, object parameters)
        {
            using (_DbCon = _ConnectionFac.CreateConnection())
            {
                List<dynamic> result = new List<dynamic>();
                var reader = _DbCon.QueryMultiple(sql, parameters);
                while (!reader.IsConsumed)
                {
                    result.Add(reader.Read());
                }

                return result.ToArray();
            }
        }

        /// <summary>
        /// SQL Procedure查詢
        /// </summary>
        /// <param name="procedureName">欲執行的Procedure</param>
        /// <param name="parameters">執行參數</param>
        /// <param name="getResults">執行後欲取得結果</param>
        /// <returns></returns>
        public Dictionary<string, object> ExcuteProcedure(string procedureName, IEnumerable<ProcedureParam> parameters, params string[] getResults)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            DynamicParameters args = new DynamicParameters();

            // *** 設定參數 ***
            foreach (var item in parameters)
            {
                args.Add(item.Name, item.Value, direction: item.Direction);
            }


            using (_DbCon = _ConnectionFac.CreateConnection())
            {
                _DbCon.Execute(procedureName, args, commandType: System.Data.CommandType.StoredProcedure);
                // *** 取得最後結果 ***
                foreach (string item in getResults)
                {
                    result.Add(item, args.Get<object>(item));
                }
            }

            return result;
        }

        /// <summary>
        /// 執行DB transaction動作
        /// </summary>
        /// <returns></returns>
        public int Excute(IEnumerable<SqlSet> settings)
        {
            int affectCount = 0;
            this._DaoTime = this.GetDbTime();
            Dictionary<string, object> parameters;
            using (_DbCon = _ConnectionFac.CreateConnection())
            {
                _DbCon.Open();
                using (var transaction = _DbCon.BeginTransaction())
                {
                    try
                    {
                        //foreach (var setting in this.Setting)
                        foreach (var setting in settings)
                        {
                            if (string.IsNullOrEmpty(setting.Sql)) continue;

                            parameters = setting.Parameters.ToDictionary();
                            parameters.Add("DaoTime", this._DaoTime);

                            affectCount += this.DbProcess(setting.Sql, parameters, _DbCon, transaction);
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return affectCount;
        }

        /// <summary>
        /// 執行DB transaction動作
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public int Excute(SqlSet setting)
        {
            int affectCount = 0;
            this._DaoTime = this.GetDbTime();
            using (_DbCon = _ConnectionFac.CreateConnection())
            {
                _DbCon.Open();
                using (var transaction = _DbCon.BeginTransaction())
                {
                    try
                    {
                        Dictionary<string, object> parameters = setting.Parameters.ToDictionary();
                        parameters.Add("DaoTime", this._DaoTime);
                        affectCount += this.DbProcess(setting.Sql, parameters, _DbCon, transaction);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return affectCount;
        }

        /// <summary>
        /// 檢核Table或Field是否存在
        /// </summary>
        /// <param name="tableName">資料表名稱</param>
        /// <param name="fieldName">欄位名稱</param>
        /// <returns></returns>
        public bool CheckTableField(string tableName, string fieldName = "")
        {
            string sql = @"SELECT CASE WHEN EXISTS( SELECT * FROM sysobjects OBJ (NOLOCK) {0}
						                             WHERE OBJ.name = @tableName {1} )
                                            THEN 'true'
                                            ELSE 'false'
                                            END".StringFormat(
                                                fieldName.IsNullOrEmpty() ? string.Empty : "INNER JOIN syscolumns COL (NOLOCK) ON OBJ.id = COL.id",
                                                fieldName.IsNullOrEmpty() ? string.Empty : "AND COL.name = @fieldName");

            return this.Query<bool>(sql, new { tableName, fieldName }).FirstOrDefault();
        }

        /// <summary>
        /// 執行SQL動作
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="parameters">SQL參數</param>
        /// <param name="dbCon"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        private int DbProcess(string sql, object parameters, IDbConnection dbCon, IDbTransaction trans)
        {
            return dbCon.Execute(sql, parameters, trans);
        }

        /// <summary>
        /// Dao取得DB時間
        /// </summary>
        /// <returns></returns>
        private DateTime GetDbTime()
        {
            return this.Query<DateTime>("SELECT GETDATE()", null).First();
        }

    }
}
