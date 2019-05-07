using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Data.Core
{
    public interface IDataReader
    {
        IEnumerable<DataSet> Get();
    }
}
