using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleData.Core
{
    public class CsvDataReader : IDataReader
    {
        public CsvDataReader(FileBatchReadOptions options)
        {
            FileBatchReadOptions = options;
        }

        public FileBatchReadOptions FileBatchReadOptions { get; }

        public IEnumerable<DataSet> Get()
        {
            var list = new List<DataSet>();
            foreach (var item in FileBatchReadOptions.Items)
            {
                list.Add(GetFromFile(item));
            }
            return list;
        }

        private DataSet GetFromFile(FileReadOptions options)
        {
            using (var sr = new StreamReader(options.FilePath, options.Encoding))
            {
                var strLine = string.Empty;

                var data = new List<IDictionary<string, object>>();
                try
                {
                    var fields = SplitCsvLine(sr.ReadLine());
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        var dict = new Dictionary<string, object>();
                        var values = SplitCsvLine(strLine);

                        var valIndex = 0;

                        for (var i = 0; i < fields.Length; i++)
                        {
                            if (string.IsNullOrEmpty(fields[i]))
                            {
                                continue;
                            }

                            var val = values[valIndex];

                            valIndex++;

                            dict.Add(fields[i], val);
                        }
                        data.Add(dict);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Read from file Error: " + ex.Message);
                }

                sr.Close();

                return new DataSet { Data = data, Name = options.DataSetName };
            }
        }

        private string[] SplitCsvLine(string line)
        {
            var list = new List<string>();

            var tempStr = line;
            var index = 0;

            while (true)
            {
                var (offset, fieldValue) = FindNextFieldValue(tempStr);
                list.Add(fieldValue);

                index += offset;

                if (offset == tempStr.Length)
                {
                    break;
                }

                tempStr = tempStr.Substring(offset + 1);
            }

            return list.ToArray();
        }

        private (int, string) FindNextFieldValue(string tempStr)
        {
            if (tempStr.StartsWith("\""))
            {
                var lastIsSoloQuationMark = false;

                var currentFieldEndPosition = -1;

                for (var indexOfTempStr = 1; indexOfTempStr < tempStr.Length; indexOfTempStr++)
                {
                    var currentChar = tempStr[indexOfTempStr];
                    if (currentChar == '"')
                    {
                        lastIsSoloQuationMark = !lastIsSoloQuationMark;
                    }
                    else if (currentChar == ',')
                    {
                        if (lastIsSoloQuationMark)
                        {
                            currentFieldEndPosition = indexOfTempStr - 1;
                            break;
                        }
                    }
                }

                if (currentFieldEndPosition == -1)
                {
                    return (tempStr.Length, EscapeCsvString(tempStr.Substring(1, tempStr.Length - 2)));
                }

                return (currentFieldEndPosition + 1, EscapeCsvString(tempStr.Substring(1, currentFieldEndPosition - 1)));
            }
            else
            {
                var indexOfFirstComma = tempStr.IndexOf(',');
                if (indexOfFirstComma == -1)
                {
                    return (tempStr.Length, EscapeCsvString(tempStr));
                }
                else
                {
                    return (indexOfFirstComma, EscapeCsvString(tempStr.Substring(0, indexOfFirstComma)));
                }
            }
        }

        private string EscapeCsvString(string str)
        {
            return str.Replace("\"\"", "\"");
        }
    }
}
