using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Data.Core
{
    public class CsvDataReader : IDataReader
    {
        public CsvDataReader(IDictionary<string, string> fileInfo, Encoding encoding = null)
        {
            FileInfo = fileInfo;
            if (encoding != null) Encoding = encoding;
            else Encoding = Encoding.Default;
        }

        public IDictionary<string, string> FileInfo { get; private set; }
        public Encoding Encoding { get; private set; }

        public IEnumerable<DataSet> Get()
        {
            var list = new List<DataSet>();
            foreach (var item in FileInfo)
            {
                list.Add(GetFromFile(item.Key, item.Value));
            }
            return list;
        }

        private DataSet GetFromFile(string dataSetName, string filePath)
        {
            StreamReader sr = new StreamReader(filePath, Encoding);
            string strLine = "";

            var data = new List<IDictionary<string, object>>();
            try
            {
                var fields = sr.ReadLine().Split(new char[] { ',' });
                while ((strLine = sr.ReadLine()) != null)
                {
                    var dic = new Dictionary<string, object>();
                    var values = strLine.Split(new char[] { ',' });
                    for (var i = 0; i < fields.Length; i++)
                    {
                        if (string.IsNullOrEmpty(fields[i])) continue;
                        dic.Add(fields[i], values[i]);
                    }
                    data.Add(dic);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("读取出错！" + ex.Message);
            }
            sr.Close();
            return new DataSet() { Data = data, Name = dataSetName };
        }
    }
}
