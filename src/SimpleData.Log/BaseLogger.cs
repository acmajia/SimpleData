using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Data.Log
{
    public abstract class BaseLogger : ILogger
    {
        public BaseLogger(LogType logMinLevel = LogType.Normal)
        {
            LogMinLevel = logMinLevel;
        }

        public LogType LogMinLevel { get; private set; }

        public abstract Task Log(Log log);

        protected Task<bool> ShouldLog(Log log)
        {
            if (log.LogType < LogMinLevel) return Task.FromResult(false);
            return Task.FromResult(true);
        }
    }
}
