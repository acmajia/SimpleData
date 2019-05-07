using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Data.Core
{
    public class JsonDataWriter : IDataWriter
    {
        public JsonDataWriter(string basePath, Encoding encoding = null)
        {
            if (basePath == null) throw new ArgumentNullException(nameof(basePath));
            BasePath = basePath;
            if (!BasePath.EndsWith("\\")) BasePath += "\\";
            if (encoding != null) Encoding = encoding;
            else Encoding = Encoding.Default;
        }

        public string BasePath { get; private set; }
        public Encoding Encoding { get; private set; }

        public void Write(IEnumerable<DataSet> dataSets)
        {
            foreach(var dataSet in dataSets)
            {
                var json = JsonConvert.SerializeObject(dataSet.Data);
                var sw = new StreamWriter($"{BasePath}{dataSet.Name}.json", false, Encoding);
                sw.Write(json);
                sw.Close();
            }
        }
    }
}
