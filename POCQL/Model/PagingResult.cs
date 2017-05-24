using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.Model
{
    public struct PagingResult<T>
    {
        public PagingResult(IEnumerable<T> datas, int total)
        {
            Datas = datas;
            Total = total;
        }

        public IEnumerable<T> Datas { get; private set; }
        public int Total { get; private set; }
    }
}
