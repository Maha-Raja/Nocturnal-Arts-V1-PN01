using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.BaseModelClass
{
    public class DataSourceResultModel<T>
    {
        public List<T> Data;
        public Int32 Total;
    }
}
