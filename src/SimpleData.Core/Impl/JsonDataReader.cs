using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleData.Data.Core
{
    public class JsonDataReader : IDataReader
    {
        public JsonDataReader(IDictionary<string, string> fileInfo, Encoding encoding = null)
        {
            FileInfo = fileInfo;
            if (encoding != null) Encoding = encoding;
            else Encoding = Encoding.UTF8;
        }

        public IDictionary<string, string> FileInfo { get; private set; }
        public Encoding Encoding { get; private set; }

        public IEnumerable<DataSet> Get()
        {
            var list = new List<DataSet>();
            foreach(var item in FileInfo)
            {
                list.Add(GetFromFile(item.Key, item.Value));
            }
            return list;
        }

        private DataSet GetFromFile(string dataSetName, string filePath)
        {
            StreamReader sr = new StreamReader(filePath, Encoding);
            string jsonStr = "";
            string[] jsonArr = { "" };
            string strLine = "";
            var data = new List<IDictionary<string, object>>();

            try
            {
                while ((strLine = sr.ReadLine()) != null)
                {
                    jsonStr += strLine;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("读取出错！" + ex.Message);
            }

            if (jsonStr.IndexOf("[") == 0 && jsonStr.LastIndexOf("]") == jsonStr.Length - 1)
            {
                jsonStr = jsonStr.Substring(2, jsonStr.Length - 3);
                Regex regex = new Regex("},{");
                jsonArr = regex.Split(jsonStr);
            }
            else
            {
                throw new Exception("错误的json格式");
            }

            for (var i = 0; i < jsonArr.Length; i++)
            {
                JsonReader reader = new JsonTextReader(new StringReader("{" + jsonArr[i] + "}"));
                JsonSerializer serializer = new JsonSerializer();
                Dictionary<string, object> dic = (Dictionary<string, object>)serializer.Deserialize(reader, typeof(Dictionary<string, object>));
                data.Add(dic);
            }
            return new DataSet() { Data = data, Name = dataSetName };
        }
    }
}
