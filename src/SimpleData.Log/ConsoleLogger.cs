using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Data.Log
{
    public class ConsoleLogger : BaseLogger
    {
        public ConsoleLogger(LogType logMinLevel = LogType.Normal) : base(logMinLevel) { }

        public override async Task Log(Log log)
        {
            if (!await ShouldLog(log)) return;
            Console.WriteLine(FormatLog(log));
        }

        private string FormatLog(Log log)
        {
            return string.Format("LogTime : {0}\r\nLogType : {1}\r\nLogInfo : {2}\r\n", log.LogTime.TimeOfDay, log.LogType.ToString(), log.LogInfo);
        }
    }
}
