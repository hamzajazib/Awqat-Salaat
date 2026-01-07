using AwqatSalaat.Configurations;
using System.Collections.Generic;

namespace AwqatSalaat.Services.CSV
{
    public class CsvRequest : RequestBase
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string FilePath { get; set; }
        public bool HasHeader { get; set; }
        public bool HasDateColumn { get; set; }
        public CsvImportRange Range { get; set; }
        public Dictionary<string, int> ColumnsMap { get; set; }
    }
}
