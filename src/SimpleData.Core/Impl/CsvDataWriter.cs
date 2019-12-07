using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Core
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
                if (!dataSet.Data.Any())
                {
                    continue;
                }

                using (var sw = new StreamWriter($"{BasePath}{dataSet.Name}.csv", true, Encoding))
                {
                    var headerSb = new StringBuilder();
                    foreach (var field in dataSet.Data.First().Keys)
                    {
                        if (headerSb.Length > 0)
                        {
                            headerSb.Append(",");
                        }
                        headerSb.Append(EscapeCsvField(field));
                    }

                    sw.WriteLine(headerSb);

                    var lineSb = new StringBuilder();
                    foreach (var row in dataSet.Data)
                    {
                        lineSb.Clear();

                        foreach (var field in row.Values)
                        {
                            if (lineSb.Length > 0)
                            {
                                lineSb.Append(",");
                            }
                            lineSb.Append(EscapeCsvField(field?.ToString()));
                        }

                        sw.WriteLine(lineSb);
                    }

                    sw.Close();
                }
            }
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                return field;
            }

            if (field.Contains("\""))
            {
                field.Replace("\"", "\"\"");
            }

            if (field.Contains(","))
            {
                field = $"\"{field}\"";
            }

            return field;
        }
    }
}
