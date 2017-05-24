using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace POCQL.Model.MapAttribute
{
    /// <summary>
    /// 多欄位Mapping
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MultiColumnMapperAttribute : Attribute
    {
        /// <summary>
        /// 欄位對應設定
        /// </summary>
        /// <param name="columns">
        /// 對應欄位名；
        /// 如果有指定資料來源，設定格式為{Column}:{DataSource}；
        /// 設定資料來源後，INSERT和UPDATE會自動忽略Property帶的值
        /// </param>
        public MultiColumnMapperAttribute(params string[] columns)   
        {
            string dataSource = columns.Where(i => Regex.IsMatch(i, @"{:\S+:}")).FirstOrDefault() ?? string.Empty;
            this.Columns = columns.Where(i => i!=dataSource).ToArray();
            this.DataSource = dataSource.Replace("{:", string.Empty).Replace(":}", string.Empty);
        }

        /// <summary>
        /// 欄位名
        /// 如果有指定資料來源，設定格式為{Column}:{DataSource}
        /// 注意: 如果設定資料來源，則在INSERT和UPDATE時會自動忽略Property帶的值
        ///       但是不影響SELECT；
        /// </summary>
        public string[] Columns { get; private set; }

        /// <summary>
        /// 同屬於這個Property的所有欄位的資料來源
        /// 如果要設定資料來源，只需要在Attribute以格式 "{:DataSource:}" 包起來即可； 
        /// 注意: 如果有給的話，不論是哪個欄位都會忽略Property帶的值，直接設定資料來源；
        ///       影響範圍只有INSERT和UPDATE，SELECT不影響；
        /// </summary>
        public string DataSource { get; private set; }
    }
}
