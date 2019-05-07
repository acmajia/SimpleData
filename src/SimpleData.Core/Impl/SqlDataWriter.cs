using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Data.Core
{
    public class SqlDataWriter : IDataWriter
    {
        public SqlDataWriter(DbConnection conn)
        {
            _conn = conn;
            ReviseString = "\"";
        }

        DbConnection _conn;

        public string ReviseString { get; set; }

        public void Write(IEnumerable<DataSet> dataSets)
        {
            var shouldClose = false;
            if (_conn.State != System.Data.ConnectionState.Open)
            {
                _conn.Open();
                shouldClose = true;

            }
            foreach (var set in dataSets)
            {
                WriteSet(set, set.Name);
            }
            if(shouldClose)
                _conn.Close();
        }

        private void WriteSet(DataSet set, string tableName)
        {
            var cmd = _conn.CreateCommand();

            foreach(var item in set.Data)
            {
                var cmdStr = string.Format("insert into {0}{1}{0}", ReviseString, tableName);
                string strField = "";
                string strValue = "";

                var index = 0;
                var count = item.Count;
                foreach (var kv in item)
                {
                    if (index < count - 1)
                    {
                        strField = string.Format("{0}{1}{2}{1}, ", strField, ReviseString, kv.Key);
                        strValue = strValue + GetItemValue(kv.Value) + ",";
                    }
                    else
                    {
                        strField = string.Format("{0}{1}{2}{1}", strField, ReviseString, kv.Key);
                        strValue = strValue + GetItemValue(kv.Value);
                    }
                    index++;
                }

                cmdStr = cmdStr + " (" + strField + ") values (" + strValue + ")";

                cmd.CommandText = cmdStr;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }

        private string GetItemValue(object value)
        {
            var str = "";
            if (value == null)
            {
                str = "null";
            }
            else
            {
                if (value.GetType() == typeof(bool))
                    str = value.ToString();
                else
                    str = "'" + value.ToString() + "'";
            }
            return str;
        }
    }
}
