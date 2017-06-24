using POCQL.Model;
using POCQL.Model.MapAttribute;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POCQL.Extension;

namespace POCQL.Process.Helper
{
    static class ConditionSetHelper
    {
        /// <summary>
        /// 根據物件的設定取得ConditionSet的資訊列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">具有ConditionMapperAttribute設定的物件</param>
        /// <param name="paramTemplate">條件參數Template，請以XXX{#PROP#}XX給</param>
        /// <returns></returns>
        public static IEnumerable<ConditionSet> GetCondition<T>(T obj, string paramTemplate = "{#PROP#}")
            where T : class
        {
            // *** 檢核Domain Model是否符合規範 ***
            return obj.ObjectAuth(entityAttr =>
            {
                var conditionSetting = obj is ICondition ?
                                       ((ICondition)obj).ConditionSetting ?? new ConditionMapperSetting()
                                       : new ConditionMapperSetting();

                // ***  取得NULL Able 白名單 ***
                var nullableProperties = obj is INullable ?
                                         new Func<IEnumerable<string>>(() =>
                                         {
                                             return ((INullable)obj).NullableProperties ?? new List<string>();
                                         })() : new string[] { };

                IEnumerable<ConditionSet> conditionSets = new ConditionSet[] { };
                ConditionAttribute attr;
                string table;
                foreach (ConditionPropertyInfo prop in obj.GetType().GetProperties().ToConditionProperty(obj, nullableProperties, conditionSetting))
                {
                    // *** 是否略過此Property不處理 ***
                    if (prop.PassToProcess) continue;

                    attr = prop.ConditionSetting;
                    table = attr.Table.IsNullOrEmpty() ? entityAttr.MainTable : attr.Table;

                    if (attr.Column.IsNullOrEmpty()) continue;

                    conditionSets += new ConditionSet(attr.Column, table, paramTemplate.Replace("{#PROP#}", prop.Name), attr.Operator, prop.Value, attr.SubQuery);
                }

                return conditionSets;
            });
        }

        /// <summary>
        /// 將ConditionSet Array轉成Dictionary
        /// </summary>
        /// <param name="conditionSets"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this IEnumerable<ConditionSet> conditionSets)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            if (conditionSets == null) return dic;
            // !!! 注意: 這裡是要將ConditionSet陣列中的參數轉成Dictionary；
            //           而傳進來的ConditionSet陣列可能有指定CustomCondition的項目，
            //           所以要先過濾掉 !!!
            foreach (ConditionSet conditionSet in conditionSets.Where(i => !i.ParameterName.IsNullOrEmpty()))
            {
                if (conditionSet.Operator != ConditionOperator.Between)
                {
                    dic.Add(conditionSet.ParameterName, conditionSet.Value);
                }
                else
                {
                    BetweenProcess(conditionSet.ParameterName, conditionSet.Value, ref dic);
                }
            }

            return dic;
        }

        /// <summary>
        /// 專屬'ConditionSet Array To Dic' Method使用，處理Between運算式
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="dic"></param>
        private static void BetweenProcess<T>(string paramName, T paramValue, ref Dictionary<string, object> dic)
            where T : class
        {
            BetweenSetAttribute attr;
            object value = null;
            string preParamName = string.Empty, postParamName = string.Empty;

            foreach (var prop in paramValue.GetType().GetProperties())
            {
                attr = prop.GetCustomAttributes(typeof(BetweenSetAttribute), true).FirstOrDefault()
                   as BetweenSetAttribute;

                if (attr == null) continue;
                value = prop.GetValue(paramValue);

                if (value == null) continue;

                // *** 從新取對應參數的名字: Pre_{Property} or Post_{Property} ***
                if (attr.Set == BetweenSet.PrefixValue)
                {
                    preParamName = attr.Set.GetDescription().StringFormat(paramName);
                    dic.AddUnrepeatKey(preParamName, value);
                }
                else
                {
                    postParamName = attr.Set.GetDescription().StringFormat(paramName);
                    dic.AddUnrepeatKey(postParamName, value);
                }
            }
        }

        /// <summary>
        /// 將conditionSets序列轉成字串
        /// </summary>
        /// <param name="conditionSets">條件資訊</param>
        /// <param name="allTables">所有Table的</param>
        /// <returns></returns>
        public static string ConvertToString(IEnumerable<ConditionSet> conditionSets, TableSet tableset)
        {
            if (conditionSets == null) return string.Empty;

            List<Tuple<string, AndOrOperator>> conditionItems = new List<Tuple<string, AndOrOperator>>();

            // *** 先取得所有Table的別名 ***
            Dictionary<string, string> tableAliases = tableset.TableAliasMap;

            string table, result;
            IEnumerable<string> cdtItem;
            ConditionSet conditionSet;

            // *** 接著取的Condition項目 ***
            foreach (var item in conditionSets.GroupBy(i => i.FullColumnName))
            {
                // *** 先取得Group中的第一筆資訊 ***
                conditionSet = item.First();
                // *** 找出Codition所屬Table ***
                table = conditionSet.TableDetailProcess(tableAliases);

                // *** 依Group分類的數量來決定是要轉成單一項目條件，
                //     或以子條件包覆，並用OR連結 ***
                cdtItem = item.ToList().Select(i => i.ConditionParse(table))
                                       .Where(i => !i.IsNullOrEmpty());

                result = cdtItem.Count() > 1 ?
                         $@"({string.Join(" OR ", cdtItem)})" :
                         cdtItem.FirstOrDefault();

                if (result.IsNullOrEmpty()) continue;
                conditionItems.Add(new Tuple<string, AndOrOperator>(result, conditionSet.AndOr));
            }

            string condition = string.Empty;
            conditionItems.ForEach(i =>
            {
                condition = string.Join(i.Item2.GetDescription(), new string[] { condition, i.Item1 }.Where(j => !j.IsNullOrEmpty()));
            });

            return condition;
        }

        /// <summary>
        /// 將conditionSets序列轉成字串
        /// </summary>
        /// <param name="conditionSets">條件資訊</param>
        /// <param name="allTables">所有Table的</param>
        /// <param name="tableMaps"></param>
        /// <returns></returns>
        public static string ConvertToString(IEnumerable<ConditionSet> conditionSets)
        {
            if (conditionSets == null) return string.Empty;

            List<Tuple<string, AndOrOperator>> conditionItems = new List<Tuple<string, AndOrOperator>>();
            string result;
            IEnumerable<string> cdtItem;
            ConditionSet conditionSet;

            // *** 接著取的Condition項目 ***
            foreach (var item in conditionSets.GroupBy(i => i.Column ?? i.CustomCondition))
            {
                // *** 先取得Group中的第一筆資訊 ***
                conditionSet = item.First();

                // *** 依Group分類的數量來決定是要轉成單一項目條件，
                //     或以子條件包覆，並用OR連結 ***
                cdtItem = item.ToList().Select(i => i.ConditionParse(string.Empty))
                                       .Where(i => !i.IsNullOrEmpty());

                result = cdtItem.Count() > 1 ?
                         $"({string.Join(" OR ", cdtItem)})" : cdtItem.FirstOrDefault();

                if (result.IsNullOrEmpty()) continue;
                conditionItems.Add(new Tuple<string, AndOrOperator>(result, conditionSet.AndOr));
            }

            string condition = string.Empty;
            conditionItems.ForEach(i =>
            {
                condition = string.Join(i.Item2.GetDescription(), new string[] { condition, i.Item1 }.Where(j => !j.IsNullOrEmpty()));
            });

            return condition;
        }
    }
}
