using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model.MapAttribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class CustomizeAttribute : Attribute
    {
        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="customizations">客製專案ID</param>
        public CustomizeAttribute(params string[] customizations) 
        {
            this.Customizations = customizations;
        }

        /// <summary>
        /// 客製專案ID
        /// </summary>
        public string[] Customizations { get; set; }
    }
}
