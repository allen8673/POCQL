using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Extension
{
    public static class DictionaryExt
    {
        /// <summary>
        /// Merge兩個Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="targetDict">目標Dictionary</param>
        /// <param name="sourceDict">來源Dictionary</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> MergeDictionary<TKey, TValue>(this Dictionary<TKey, TValue> targetDict, Dictionary<TKey, TValue> sourceDict)
        {
            return targetDict.Union(sourceDict.Where(x => !targetDict.Keys.Contains(x.Key))).ToDictionary(k => k.Key, v => v.Value);
        }

        /// <summary>
        /// 新增未重複Key的資料
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict">目標Dictionary</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void AddUnrepeatKey<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (key == null || key.ToString() == "") return;
            if (!dict.ContainsKey(key)) dict.Add(key, value);
        }
    }
}
