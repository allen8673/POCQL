using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.ToolExt
{
    public static class DictionaryExt
    {
        public static Dictionary<TKey, TValue> MergeDictionary<TKey, TValue>(this Dictionary<TKey, TValue> targetDict, Dictionary<TKey, TValue> sourceDict)
        {
            return targetDict.Union(sourceDict.Where(x => !targetDict.Keys.Contains(x.Key))).ToDictionary(k => k.Key, v => v.Value);
        }

        public static void AddUnrepeatKey<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (key == null || key.ToString() == "") return;
            if (!dict.ContainsKey(key)) dict.Add(key, value);
        }
    }
}
