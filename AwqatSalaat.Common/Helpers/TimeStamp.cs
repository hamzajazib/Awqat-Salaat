using Serilog;
using System;
using System.Threading;

namespace AwqatSalaat.Helpers
{
    public static class TimeStamp
    {
        private static readonly Timer s_timer = new Timer(InternalTimerTick, null, 0, 1000);

        private static DateTime s_currentDate = DateTime.MinValue;

        public static DateTime Now
        {
            get
            {
#if DEBUG
                return new DateTime(2023, 9, 13, 16, 00, 00);
#else
                return DateTime.Now;
#endif
            }
        }

        public static DateTime Date => Now.Date;
        public static DateTime NextDate => Date.AddDays(1);

        public static event Action DateChanged;
        public static event Action TimerTick;

        private static void InternalTimerTick(object state)
        {
            TimerTick?.Invoke();

            var today = Now.Date;

            if (today != s_currentDate)
            {
                bool skipEvent = s_currentDate == DateTime.MinValue;
                s_currentDate = today;

                if (!skipEvent)
                {
                    Log.Information($"Date changed to {today:dd/MM/yyyy}");
                    DateChanged?.Invoke();
                }
            }
        }
    }
}