using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Data.Log
{
    public class FileLogger : BaseLogger
    {
        public FileLogger(string logDirectoryPath, LogType logMinLevel = LogType.Normal) : base(logMinLevel)
        {
            LogDirectoryPath = logDirectoryPath;
        }

        private static object _syncObject = new object();

        public string LogDirectoryPath { get; private set; }

        public override async Task Log(Log log)
        {
            if (!await ShouldLog(log)) return;
            var now = DateTime.Now;
            var filePath = string.Format("{0}\\{1}-{2}-{3}.txt", LogDirectoryPath, now.Year, now.Month, now.Day);
            var file = new StreamWriter(filePath, true, Encoding.Default);
            lock (_syncObject)
            {
                file.Write(FormatLog(log));
                file.Flush();
                file.Close();
            }
        }

        private string FormatLog(Log log)
        {
            return string.Format("LogTime : {0}\r\nLogType : {1}\r\nLogInfo : {2}\r\n", log.LogTime.TimeOfDay, log.LogType.ToString(), log.LogInfo);
        }
    }
}
