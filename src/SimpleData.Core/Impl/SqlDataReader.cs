using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Core
{
    public class SqlDataReader : IDataReader
    {
        public SqlDataReader(DbConnection conn, IDictionary<string, string> queryCmds)
        {
            _conn = conn;
            _queryCmds = queryCmds;
        }

        DbConnection _conn;
        IDictionary<string, string> _queryCmds;

        public IEnumerable<DataSet> Get()
        {
            var dataSets = new List<DataSet>();

            foreach(var item in _queryCmds)
            {
                dataSets.Add(new DataSet()
                {
                    Name = item.Key,
                    Data = Get(item.Value)
                });
            }
            return dataSets;
        }

        private IEnumerable<IDictionary<string, object>> Get(string cmdStr)
        {
            _conn.Open();
            var result = new List<IDictionary<string, object>>();
            var cmd = _conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = cmdStr;
            try
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var dicItem = new Dictionary<string, object>();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var fieldName = reader.GetName(i);
                        var fieldValue = (reader.GetValue(i).GetType().ToString() == "System.DBNull") ? null : reader.GetValue(i);
                        dicItem.Add(fieldName, fieldValue);
                    }
                    result.Add(dicItem);
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _conn.Close();
            }
        }
    }
}
