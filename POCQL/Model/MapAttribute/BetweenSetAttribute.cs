using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model.MapAttribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class BetweenSetAttribute : Attribute
    {
        public BetweenSetAttribute(BetweenSet set) 
        {
            this.Set = set;
        }

        public BetweenSet Set { get; private set; }
    }
}
