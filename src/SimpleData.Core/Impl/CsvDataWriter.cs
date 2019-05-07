using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Data.Core
{
    public class CsvDataWriter : IDataWriter
    {
        public CsvDataWriter(string basePath, Encoding encoding = null)
        {
            if (basePath == null) throw new ArgumentNullException(nameof(basePath));
            BasePath = basePath;
            if (!BasePath.EndsWith("\\")) BasePath += "\\";
            if (encoding != null) Encoding = encoding;
            else Encoding = Encoding.Default;
        }

        public string BasePath { get;  private set; }
        public Encoding Encoding { get; private set; }

        public void Write(IEnumerable<DataSet> dataSets)
        {
            foreach (var dataSet in dataSets)
            {
                var sw = new StreamWriter($"{BasePath}{dataSet.Name}.csv", true, Encoding);

                var dataStr = "";
                var i = 0;
                foreach (var item in dataSet.Data)
                {
                    if (i == 0)
                    {
                        var j = 0;
                        foreach (var piece in item.Keys)
                        {
                            dataStr += piece;
                            if (j < item.Keys.Count - 1)
                                dataStr += ",";
                            j++;
                        }
                    }
                    dataStr += "\r\n";
                    var k = 0;
                    foreach (var piece in item)
                    {
                        dataStr += piece.Value;
                        if (k < item.Keys.Count - 1)
                            dataStr += ",";
                        k++;
                    }
                    i++;
                }

                sw.Write(dataStr);
                sw.Close();
            }
        }
    }
}
