using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model
{
    public class OrderSet : Dictionary<string, SortKind>
    {
        public OrderSet() { }
        public OrderSet(IDictionary<string, string> dic)
            : base(dic.ToDictionary(i => i.Key, j => j.Value.ToEnum<SortKind>()))
        { }

        public void Add(string key)
        {
            base.Add(key, SortKind.ASC);
        }
    }
}
