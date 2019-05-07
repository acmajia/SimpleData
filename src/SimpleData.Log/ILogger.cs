using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Data.Log
{
    public interface ILogger
    {
        Task Log(Log log);
    }
}
