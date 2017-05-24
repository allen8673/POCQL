using POCQL.Model;
using POCQL.Model.MapAttribute;
using POCQL.SqlObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using POCQL.Model.Interface;
using POCQL.Process.Helper;
using POCQL.ToolExt;

namespace POCQL
{
    static class Utility
    {
        /// <summary>
        /// 取得ColumnSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapProperty">
        ///     是否轉換結果的名字，如果要，最後會以{Column} AS {Property Name}輸出結果
        /// </param>
        /// <returns></returns>
        public static IEnumerable<ColumnSet> GetColumns<T>(bool mapProperty)
            where T : class
        {
            return ColumnSetHelper.GetColumnSets(Activator.CreateInstance<T>(), true, mapProperty);
        }

        /// <summary>
        /// 取得物件Property對應的Column資訊，不包含Property的值
        /// </summary>
        /// <typeparam name="TDomain">Domain Model Type</typeparam>
        /// <typeparam name="TView">View Model Type</typeparam>
        /// <param name="mapProperty">是否Map至Property</param>
        /// <param name="domainProperties">區域物件以外，額外指定的全域物件Property</param>
        /// <returns></returns>
        public static IEnumerable<ColumnSet> GetColumns<TDomain, TView>(bool mapProperty)
            where TDomain : class
        {
            // *** 如果Domain Model Type等於View Model Type，
            //     則不需要再做兩個物件的Property Map，
            //     直接取得Domain Model對應的Column資訊       ***
            if (typeof(TView) == typeof(TDomain))
                return ColumnSetHelper.GetColumnSets(Activator.CreateInstance<TDomain>(), true, mapProperty);

            return ColumnSetHelper.GetColumnSets(Activator.CreateInstance<TDomain>(),
                                                 Activator.CreateInstance<TView>(),
                                                 true, mapProperty);

        }

        /// <summary>
        /// 依指定Match值，取得與Match值相似的Column資訊，包含Property的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matche">指定Match值</param>
        /// <returns></returns>
        public static IEnumerable<ColumnSet> GetMatchColumns<T>(string matche)
            where T : class
        {
            return ColumnSetHelper.GetMatchColumnSets(Activator.CreateInstance<T>(), true, matche);
        }

        /// <summary>
        /// 取得物件Property對應的Column資訊，包含Property的值
        /// </summary>
        /// <param name="obj">全域物件(Domain Model)</param>
        /// <param name="mapProperty">是否Map至Property</param>
        /// <returns></returns>
        public static IEnumerable<ColumnSet> GetColumnValues<T>(T obj, bool mapProperty)
            where T : class
        {
            return ColumnSetHelper.GetColumnSets(obj, false, mapProperty);
        }

        /// <summary>
        /// 依指定Match值，取得與Match值相似的Column資訊，包含Property的值
        /// </summary>
        /// <param name="obj">全域物件(Domain Model)</param>
        /// <param name="matches">指定Match值</param>
        /// <returns></returns>
        public static IEnumerable<ColumnSet> GetMatchColumnValues<T>(T obj, params string[] matches)
            where T : class
        {
            return ColumnSetHelper.GetMatchColumnSets(obj, false, matches);
        }

        /// <summary>
        /// 依指定Match值，取得與Match值相似的Column資訊，不包含Property的值
        /// </summary>
        /// <param name="obj">全域物件(Domain Model)</param>
        /// <param name="matches">指定Match值</param>
        /// <returns></returns>
        public static IEnumerable<ColumnSet> GetMatchColumns<T>(T obj, params string[] matches)
            where T : class
        {
            return ColumnSetHelper.GetMatchColumnSets(obj, true, matches);
        }

        /// <summary>
        /// 將conditionSets序列轉成字串
        /// </summary>
        /// <param name="conditionSets">條件資訊</param>
        /// <param name="allTables">所有Table的</param>
        /// <returns></returns>
        public static string ConvertToString(this IEnumerable<ConditionSet> conditionSets, TableSet table)
        {
            return ConditionSetHelper.ConvertToString(conditionSets, table);
        }

        /// <summary>
        /// 將conditionSets序列轉成字串
        /// </summary>
        /// <param name="conditionSets">條件資訊</param>
        /// <returns></returns>
        public static string ConvertToString(this IEnumerable<ConditionSet> conditionSets)
        {
            return ConditionSetHelper.ConvertToString(conditionSets);
        }

        /// <summary>
        /// 輸出實作IColumnObject的物件的值，
        /// 通常用來當INSERT或UPDATE TABLE時的參數用。
        /// </summary>
        /// <param name="obj">實作IColumnObject的物件</param>
        /// <returns></returns>
        public static object OutputParameters<T>(BaseObject<T> obj, Dictionary<string, object> tempValue)
            where T : class
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            // *** 取得IColumnObject的參數 ***
            foreach (var item in obj.ColumnInfos.Where(i => !i.ReadOnly))
            {
                // *** ColumnSet如果有指定DataSource，
                //     則代表這個ColumnSet的值已經有指定來源，
                //     不應該從ColumnSet的Value給 ***
                if (item.DataSource != null) continue;
                dic.AddUnrepeatKey(item.ColumnName, item.Value);
            }

            dic = dic.MergeDictionary(tempValue);

            return dic;
        }

        /// <summary>
        /// 輸出實作IColumnObject的物件的值，
        /// 通常用來當INSERT或UPDATE TABLE時的參數用。
        /// </summary>
        /// <param name="obj">實作IColumnObject的物件</param>
        /// <param name="tempValue">物件的暫存值</param>
        /// <param name="otherParas">其他指定參數</param>
        /// <returns></returns>
        public static object OutputParameters<T>(BaseObject<T> obj, Dictionary<string, object> tempValue, object otherParas)
            where T : class
        {
            // *** 取得IColumnObject的參數 ***
            Dictionary<string, object> dic = Utility.OutputParameters<T>(obj, tempValue) as Dictionary<string, object>;

            // *** 整合其他參數，並且回傳 ***
            return dic.MergeDictionary(otherParas.ToDictionary());
        }

        /// <summary>
        /// 輸出實作IColumnObject的物件的值，
        /// 通常用來當INSERT或UPDATE TABLE時的參數用。
        /// </summary>
        /// <param name="tempValue">物件的暫存值</param>
        /// <param name="otherParas">其他指定參數</param>
        /// <returns></returns>
        public static object OutputParameters(Dictionary<string, object> tempValue, object otherParas)
        {
            // *** 整合其他參數 ***
            return tempValue.MergeDictionary(otherParas.ToDictionary());
        }

        /// <summary>
        /// 依Table資訊取得相對應的Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">實作ITableDetail的物件</param>
        /// <param name="table2Alias">Table別名</param>
        /// <returns></returns>
        public static string TableDetailProcess<T>(this T obj, Dictionary<string, string> table2Alias)
            where T : ITableDetail
        {
            // *** 如果有Table對應的別名就回傳別名，否則就回傳原本TableName
            return table2Alias.ContainsKey(obj.TableName??string.Empty) ?
                    table2Alias[obj.TableName] :
                    obj.TableName;
        }

        /// <summary>
        /// SqlSet轉型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlSet">SqlSet物件</param>
        /// <returns></returns>
        public static T ToType<T>(this SqlSet sqlSet)
        {
            Type findType = sqlSet.GetType();
            PropertyInfo findProperty;
            T toType = (T)Activator.CreateInstance(typeof(T));

            foreach (var prop in typeof(T).GetProperties())
            {
                findProperty = findType.GetProperty(prop.Name);
                if (findProperty == null) continue;
                prop.SetValue(toType, findProperty.GetValue(sqlSet));
            }

            return toType;
        }

        /// <summary>
        /// SqlSet陣列轉型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlSets">SqlSet物件陣列</param>
        /// <returns></returns>
        public static IEnumerable<T> ToType<T>(this IEnumerable<SqlSet> sqlSets)
        {
            List<T> toTypes = new List<T>();

            foreach (SqlSet sqlSet in sqlSets)
            {
                toTypes.Add(sqlSet.ToType<T>());
            }

            return toTypes;
        }
    }
}
