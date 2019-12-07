using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleData.Core
{
    public class FileReadOptions
    {
        public string DataSetName { get; set; }
        public string FilePath { get; set; }
        public Encoding Encoding { get; set; } = Encoding.UTF8;
    }

    public class FileBatchReadOptions
    {
        public IEnumerable<FileReadOptions> Items { get; set; }
    }
}
