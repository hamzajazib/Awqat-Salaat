using CsvHelper.Configuration;
using System.Collections.Generic;

namespace AwqatSalaat.Services.CSV
{
    internal class CsvTimesMap : ClassMap<CsvTimes>
    {
        public CsvTimesMap(Dictionary<string, int> map)
        {
            Map(t => t.Fajr).Index(map[nameof(CsvTimes.Fajr)]);
            Map(t => t.Shuruq).Index(map[nameof(CsvTimes.Shuruq)]);
            Map(t => t.Dhuhr).Index(map[nameof(CsvTimes.Dhuhr)]);
            Map(t => t.Asr).Index(map[nameof(CsvTimes.Asr)]);
            Map(t => t.Maghrib).Index(map[nameof(CsvTimes.Maghrib)]);
            Map(t => t.Isha).Index(map[nameof(CsvTimes.Isha)]);

            if (map.ContainsKey(nameof(CsvTimes.Date)))
            {
                Map(t => t.Date).Index(map[nameof(CsvTimes.Date)]);
            }
            else if (map.ContainsKey(nameof(CsvTimes.Day)) && map.ContainsKey(nameof(CsvTimes.Month)))
            {
                Map(t => t.Day).Index(map[nameof(CsvTimes.Day)]);
                Map(t => t.Month).Index(map[nameof(CsvTimes.Month)]);
            }
        }
    }
}
