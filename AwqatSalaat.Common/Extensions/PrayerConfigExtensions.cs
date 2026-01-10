using AwqatSalaat.Configurations;

namespace AwqatSalaat.Extensions
{
    public static class PrayerConfigExtensions
    {
        public static ushort EffectiveReminderOffset(this PrayerConfig config)
        {
            return config.GlobalReminderOffset
                ? Properties.Settings.Default.NotificationDistance
                : config.ReminderOffset;
        }

        public static byte EffectiveElapsedTime(this PrayerConfig config)
        {
            return config.GlobalElapsedTime
                ? Properties.Settings.Default.NotificationDistanceElapsed
                : config.ElapsedTime;
        }

        public static string EffectiveAdhanFile(this PrayerConfig config)
        {
            return !config.StandardAdhan
                ? config.AdhanFile
                : config.Key == nameof(Data.PrayerTimes.Fajr)
                    ? Properties.Settings.Default.AdhanFajrSoundFilePath
                    : Properties.Settings.Default.AdhanSoundFilePath;
        }
    }
}
