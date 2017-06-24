using POCQL.Extension;
using POCQL.Model;
using POCQL.Model.MapAttribute;
using POCQL.MSSQL;
using POCQL.Process.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Helper
{
    public static class SqlHelper
    {
        /// <summary>
        /// 重物件中取得符合Match String的欄位群組；
        /// Ex:Match String = 'CTR' & 'MDF'，回傳{{CRT_COMP_ID:MDF_USR_ID} {CRT_USR_ID:MDF_USR_ID} ...} 
        /// </summary>
        /// <param name="obj">指定物件，限定為具有</param>
        /// <param name="matchs">Match String</param>
        /// <returns></returns>
        public static string[] GetMatchColumnsGroups<T>(T obj, params string[] matchs)
            where T : class
        {
            return GetMatchColumnsGroupsProcess(obj, matchs).ToArray();
        }

        /// <summary>
        /// 重物件中取得符合Match String的欄位群組；
        /// Ex:Match String = 'CTR' & 'MDF'，回傳{{CRT_COMP_ID:MDF_USR_ID} {CRT_USR_ID:MDF_USR_ID} ...} 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matchs">Match String</param>
        /// <returns></returns>
        public static string[] GetMatchColumnsGroups<T>(params string[] matchs)
            where T : class
        {
            return GetMatchColumnsGroupsProcess(Activator.CreateInstance<T>(), matchs).ToArray();
        }

        /// <summary>
        /// 【GetMatchColumnsGroups】處理程序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">指定物件，限定為具有</param>
        /// <param name="matchs">Match String</param>
        /// <returns></returns>
        private static IEnumerable<string> GetMatchColumnsGroupsProcess<T>(T obj, params string[] matchs)
            where T : class
        {
            ColumnSet[] result = Utility.GetMatchColumns(obj, matchs).ToArray();
            IEnumerable<string> group;

            foreach (string mapToProp in result.Select(i => i.Property).Distinct())
            {
                group = result.Where(i => i.Property == mapToProp && matchs.Any(match => i.ColumnName.StartsWith(match)))
                              .Select(i => i.ColumnName);

                if (group.Count() == matchs.Count()) yield return string.Join(":", group);
                // *** 排除已經找尋過的Property，以利後面加快速度 ***
                result = result.Where(i => i.Property != mapToProp).ToArray();
            }

        }

        /// <summary>
        /// 依指定Match值，取得與Match值相似的Column資訊，包含Property的值
        /// </summary>
        /// <param name="obj">全域物件(Domain Model)</param>
        /// <param name="matches">指定Match值</param>
        /// <returns></returns>
        public static Dictionary<string, object> GetMatchColumnValue(object obj, params string[] matches)
        {
            IEnumerable<ColumnSet> columns = Utility.GetMatchColumnValues(obj, matches);
            return columns.ToDictionary(k => k.ColumnName, v => v.Value);
        }

        /// <summary>
        /// 解析條件物件，並取得條件物件的參數
        /// </summary>
        /// <param name="obj">條件物件</param>
        /// <param name="includeOtherPorp">其他未設定ConditionAttribute的Property是否要一併解析取值</param>
        /// <returns></returns>
        public static Dictionary<string, object> ParseConditionValue<T>(T obj, bool includeOtherPorp)
            where T : class
        {
            Dictionary<string, object> result = ConditionSetHelper.GetCondition(obj).ToDictionary();

            // *** 解析取得其他未設定ConditionAttribute的Property的值 ***
            if (!includeOtherPorp) return result;

            ConditionAttribute[] attrs;
            object propValue = null;

            foreach (var prop in obj.GetType().GetProperties())
            {
                // *** 是否有 ConditionMapperAttribute ***
                attrs = prop.GetCustomAttributes(typeof(ConditionAttribute), true) as ConditionAttribute[];
                propValue = prop.GetValue(obj);

                //找不到，或 Column 值為空則跳過
                if (attrs.Count() > 0 || propValue == null) continue;
                result.Add(prop.Name, propValue);
            }

            return result;
        }

        /// <summary>
        /// 解析條件物件，並取得條件物件的參數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">條件物件</param>
        /// <param name="otherValue">其他要一同加入的參數</param>
        /// <returns></returns>
        public static Dictionary<string, object> ParseConditionValue<T>(T obj, object otherParams)
            where T : class
        {
            Dictionary<string, object> result = ConditionSetHelper.GetCondition(obj).ToDictionary();
            return result.MergeDictionary(otherParams.ToDictionary());
        }

        /// <summary>
        /// 解析條件物件，並取得條件物件的參數
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">條件物件</param>
        /// <param name="paramTemplate"></param>
        /// <param name="includeOtherPorp">其他未設定ConditionAttribute的Property是否要一併解析取值</param>
        /// <returns></returns>
        public static Dictionary<string, object> ParseConditionValue<T>(T obj, string paramTemplate, bool includeOtherPorp)
            where T : class
        {
            return ParseConditionValue(obj, includeOtherPorp).ToDictionary(k => paramTemplate.Replace("{#PROP#}", k.Key), v => v.Value);
        }

        /// <summary>
        /// 映射物件Property與條件式
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="obj">欲映射的物件</param>
        /// <param name="propertyMap">Lambda Expression</param>
        /// <param name="column">映射目標Column</param>
        /// <param name="conditionOpt">條件運算子</param>
        /// <returns></returns>
        public static ConditionSet ConditionMap<TObj>(this TObj obj, Expression<Func<TObj, object>> propertyMap, string column, ConditionOperator conditionOpt)
            where TObj : class
        {
            return obj.ObjectAuth(attri => ConditionMap<TObj>(obj, propertyMap, attri.MainTable, column, conditionOpt));
        }

        /// <summary>
        /// 映射物件Property與條件式
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="obj">欲映射的物件</param>
        /// <param name="propertyMap">Lambda Expression</param>
        /// <param name="table">映射目標Table</param>
        /// <param name="column">映射目標Column</param>
        /// <param name="conditionOpt">條件運算子</param>
        /// <returns></returns>
        public static ConditionSet ConditionMap<TObj>(this TObj obj, Expression<Func<TObj, object>> propertyMap,
                                                      string table, string column, ConditionOperator conditionOpt)
            where TObj : class
        {
            // *** 取得Domain Property Name ***
            string propName = propertyMap.GetPropertyName();

            // *** 檢核指定的Property是否有值 ***
            // *** 如果Property Value為null，就直接回傳空物件
            object value = typeof(TObj).GetProperty(propName).GetValue(obj);

            if (value == null) return null;

            return new ConditionSet(column, table, propName, conditionOpt, value, string.Empty);
        }

        /// <summary>
        /// 映射物件Property與條件式
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="obj">欲映射的物件</param>
        /// <param name="propertyMap">Lambda Expression</param>
        /// <param name="table">映射目標Table</param>
        /// <param name="column">映射目標Column</param>
        /// <param name="conditionOpt">條件運算子</param>
        /// <param name="subSelect">子查詢</param>
        /// <returns></returns>
        public static ConditionSet ConditionMap<TObj>(this TObj obj, Expression<Func<TObj, object>> propertyMap,
                                                      string table, string column, ConditionOperator conditionOpt, SelectObject subSelect)
            where TObj : class
        {
            // *** 取得Domain Property Name ***
            string propName = propertyMap.GetPropertyName();

            // *** 檢核指定的Property是否有值 ***
            // *** 如果Property Value為null，就直接回傳空物件
            object value = typeof(TObj).GetProperty(propName).GetValue(obj);

            if (value == null) return null;

            return new ConditionSet(column, table, propName, conditionOpt, value, subSelect.ToString());
        }

        /// <summary>
        /// 映射物件Property與條件式
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="obj">欲映射的物件</param>
        /// <param name="propertyMap">Lambda Expression</param>
        /// <param name="column">映射目標Column</param>
        /// <param name="conditionOpt">條件運算子</param>
        /// <param name="subSelect">子查詢</param>
        /// <returns></returns>
        public static ConditionSet ConditionMap<TObj>(this TObj obj, Expression<Func<TObj, object>> propertyMap,
                                                      string column, ConditionOperator conditionOpt, SelectObject subSelect)
            where TObj : class
        {
            return obj.ObjectAuth(attr => ConditionMap<TObj>(obj, propertyMap, attr.MainTable, column, conditionOpt, subSelect));
        }

        /// <summary>
        /// 映射物件Property與條件式
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="obj">欲映射的物件</param>
        /// <param name="propertyMap">Lambda Expression</param>
        /// <param name="table">映射目標Table</param>
        /// <param name="column">映射目標Column</param>
        /// <param name="subSelect">子查詢</param>
        /// <returns></returns>
        public static ConditionSet ConditionMap<TObj>(this TObj obj, Expression<Func<TObj, object>> propertyMap,
                                                      string table, string column, SelectObject subSelect)
            where TObj : class

        {
            return ConditionMap<TObj>(obj, propertyMap, table, column, ConditionOperator.Equal, subSelect);
        }

        /// <summary>
        /// 映射物件Property與條件式
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="obj">欲映射的物件</param>
        /// <param name="propertyMap">Lambda Expression</param>
        /// <param name="column">映射目標Column</param>
        /// <param name="subSelect">子查詢</param>
        /// <returns></returns>
        public static ConditionSet ConditionMap<TObj>(this TObj obj, Expression<Func<TObj, object>> propertyMap,
                                                       string column, SelectObject subSelect)
            where TObj : class
        {
            return ConditionMap<TObj>(obj, propertyMap, column, ConditionOperator.Equal, subSelect);
        }
    }
}
