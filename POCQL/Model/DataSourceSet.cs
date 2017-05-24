using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model
{
    public class DataSourceSet
    {
        public DataSourceSet(string value) 
        {
            this.SourceValue = value;
        }

        public DataSourceSet(string value, string table)
            :this(value)
        {
            this.SourceTable = table;
        }

        public string SourceValue { get; private set; }
        public string SourceTable { get; private set; }

    }
}
