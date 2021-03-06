﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Extension
{
    public static class ArrayExt
    {
        /// <summary>
        /// 增加Item至Array物件中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">Array</param>
        /// <param name="parms">新增項目</param>
        /// <returns></returns>
        public static T[] Append<T>(this T[] array, params T[] parms)
        {
            if (parms == null) return array;

            List<T> list = array.ToList();
            foreach (T parm in parms.Where(i => i != null))
            {
                list.Add(parm);
            }
            return list.ToArray();
        }
    }
}
