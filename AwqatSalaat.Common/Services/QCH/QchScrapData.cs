using System;
using System.Linq;

namespace AwqatSalaat.Services.QCH
{
    public class QchScrapData
    {
        public QchPrayData[] PrayData { get; }
        public QchCalendar Calendar { get; }
        public DateTime Date { get; }

        public QchScrapData(QchPrayData[] prayData, QchCalendar calendar)
        {
            PrayData = prayData;
            Calendar = calendar;

            var day = calendar.Months.SelectMany(x => x.Days).Single(d => d.Today);
            var dayInMonth = int.Parse(day.Gregorian);
            var date = DateTime.Now.Date;

            if (dayInMonth == date.Day)
            {
                Date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Unspecified);
            }
            else if (dayInMonth == date.AddDays(1).Day)
            {
                date = date.AddDays(1);
                Date = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Unspecified);
            }
            else
            {
                throw new QchException("Unexpected date");
            }
        }
    }
}
