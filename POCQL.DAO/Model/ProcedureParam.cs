using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.DAO.Model
{
    public class ProcedureParam
    {
        public ProcedureParam(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public ProcedureParam(string name, string value, ParameterDirection direction)
            : this(name, value)
        {
            this.Direction = direction;
        }


        public string Name { get; set; }
        public object Value { get; set; }
        public ParameterDirection? Direction { get; set; }
    }
}
