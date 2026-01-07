using AwqatSalaat.Configurations;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace AwqatSalaat.Services.CSV
{
    internal class CsvClient : IServiceClient
    {
        public bool SupportMonthlyData => true;

        public Task<ServiceData> GetDataAsync(IRequest request)
        {
            var req = (CsvRequest)request;

            try
            {
                using (var reader = new StreamReader(req.FilePath))
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = req.HasHeader
                    };
                    using (var csv = new CsvReader(reader, config))
                    {
                        csv.Context.RegisterClassMap(new CsvTimesMap(req.ColumnsMap));

                        var dict = new Dictionary<DateTime, Data.PrayerTimes>();

                        if (req.HasDateColumn)
                        {
                            int counter = 0;

                            while (csv.Read())
                            {
                                if (counter > 366)
                                {
                                    throw new InvalidOperationException("CSV file contains more rows than expected");
                                }

                                var record = csv.GetRecord<CsvTimes>();
                                var date = DateTime.MinValue;

                                try
                                {
                                    if (string.IsNullOrEmpty(record.Date))
                                    {
                                        date = new DateTime(DateTime.Today.Year, int.Parse(record.Month), int.Parse(record.Day));
                                    }
                                    else
                                    {
                                        date = DateTime.Parse(record.Date);
                                    }
                                }
                                catch
                                {
                                    continue;
                                }
                                finally
                                {
                                    counter++;
                                }

                                if (req.Date.Year == date.Year && req.Date.Month == date.Month)
                                {
                                    if (req.GetEntireMonth || req.Date.Day == date.Day)
                                    {
                                        dict.Add(date, GetPrayerTimes(record, date));

                                        if (!req.GetEntireMonth)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var dates = req.Range == CsvImportRange.Year ? GetYearDates() : GetMonthDates();

                            foreach (var date in dates)
                            {
                                if (date.Month > req.Date.Month || !csv.Read())
                                {
                                    break;
                                }

                                var record = csv.GetRecord<CsvTimes>();

                                if (req.Date.Month == date.Month)
                                {
                                    if (req.GetEntireMonth || req.Date.Day == date.Day)
                                    {
                                        dict.Add(date, GetPrayerTimes(record, date));

                                        if (!req.GetEntireMonth)
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        return Task.FromResult(new ServiceData { Times = dict });
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static Data.PrayerTimes GetPrayerTimes(CsvTimes csvTimes, DateTime date)
        {
            var times = new Data.PrayerTimes(new Dictionary<string, DateTime>
            {
                [nameof(Data.PrayerTimes.Fajr)] = ParseTimeToLocal(csvTimes.Fajr, date),
                [nameof(Data.PrayerTimes.Shuruq)] = ParseTimeToLocal(csvTimes.Shuruq, date),
                [nameof(Data.PrayerTimes.Dhuhr)] = ParseTimeToLocal(csvTimes.Dhuhr, date),
                [nameof(Data.PrayerTimes.Asr)] = ParseTimeToLocal(csvTimes.Asr, date),
                [nameof(Data.PrayerTimes.Maghrib)] = ParseTimeToLocal(csvTimes.Maghrib, date),
                [nameof(Data.PrayerTimes.Isha)] = ParseTimeToLocal(csvTimes.Isha, date),
            });

            return times;
        }

        private static DateTime ParseTimeToLocal(string time, DateTime baseDate)
        {
            var dt = baseDate + DateTime.Parse(time, CultureInfo.InvariantCulture).TimeOfDay;

            return TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Local);
        }

        private static IEnumerable<DateTime> GetYearDates()
        {
            int year = DateTime.Today.Year;
            var date = new DateTime(year, 1, 1);

            while (date.Year == year)
            {
                yield return date;
                date = date.AddDays(1);
            }
        }

        private static IEnumerable<DateTime> GetMonthDates()
        {
            int year = DateTime.Today.Year;
            int month = DateTime.Today.Month;
            var date = new DateTime(year, month, 1);

            while (date.Month == month)
            {
                yield return date;
                date = date.AddDays(1);
            }
        }
    }
}
