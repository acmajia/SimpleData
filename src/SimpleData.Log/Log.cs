using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Data.Log
{
    public class Log
    {
        public Log()
        {
            LogType = LogType.Normal;
            LogTime = DateTime.Now;
        }
        public Log(string logInfo, LogType logType = LogType.Normal, DateTime? logTime = null)
        {
            LogType = logType;
            LogTime = logTime ?? DateTime.Now;
            LogInfo = logInfo;
        }

        public LogType LogType { get; set; }
        public DateTime LogTime { get; set; }
        public string LogInfo { get; set; }
    }

    public enum LogType
    {
        Normal = 0,
        Warning = 1,
        Error = 2
    }
}

