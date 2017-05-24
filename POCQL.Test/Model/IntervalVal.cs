using POCQL.Model.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Test.Example.Model
{
    /// <summary>
    /// 專屬於BETWEEN條件運算子的Model
    /// </summary>
    public class IntervalVal
    {
        public IntervalVal(string pre, string post) 
        {
            this.PrefixValue = pre;
            this.PostfixValue = post;
        }

        [BetweenSet(BetweenSet.PrefixValue)]
        public string PrefixValue { get; set; }

        [BetweenSet(BetweenSet.PostfixValue)]
        public string PostfixValue { get; set; }
    }
}
