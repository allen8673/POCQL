using POCQL.ToolExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model.MapAttribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AggregationAttribute : Attribute
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="function">Aggregate Function</param>
        public AggregationAttribute(AggregateFunction function)
        {
            this.Function = function;
        }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="function">Aggregate Function</param>
        /// <param name="content">Aggregate Function指定內容，如果包含COLUMN，於字串中以{#COLUMN#}代表COLUMN的設定位置</param>
        public AggregationAttribute(AggregateFunction function, string content)
            : this(function)
        {
            this.FunctionContent = content;
        }

        /// <summary>
        /// Aggregate Function
        /// </summary>
        public string AggregateFunction
        {
            get
            {
                return this.FunctionContent.IsNullOrEmpty() ? 
                       this.Function.GetDescription() :
                       this.Function.GetDescription().Replace("{#COLUMN#}", this.FunctionContent);
            }
        }

        /// <summary>
        /// Aggregate Function
        /// </summary>
        private AggregateFunction Function { get; set; }

        /// <summary>
        /// Aggregate Function指定內容，如果包含COLUMN，於字串中以{#COLUMN#}代表COLUMN的設定位置
        /// </summary>
        private string FunctionContent { get; set; }
    }
}
