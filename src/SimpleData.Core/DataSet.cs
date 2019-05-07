using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Data.Core
{
    public class DataSet
    {
        public string Name { get; set; }
        public IEnumerable<IDictionary<string, object>> Data { get; set; }
    }
}
