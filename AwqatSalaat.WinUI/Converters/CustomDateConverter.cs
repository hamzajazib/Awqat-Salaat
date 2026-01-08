using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AwqatSalaat.WinUI.Converters
{
    internal class CustomDateTimeConverter : IValueConverter
    {
        public static string Format(DateTime dateTime, string format, string language)
        {
            CultureInfo culture = string.IsNullOrEmpty(language) ? CultureInfo.CurrentUICulture : CultureInfo.GetCultureInfo(language);
            return dateTime.ToString(format, culture);
        }

        private static string GetShortPattern()
        {
            var shortPattern = Properties.Settings.Realtime.ShortTimePattern;

            if (string.IsNullOrEmpty(shortPattern))
            {
                shortPattern = CultureInfo.InstalledUICulture.DateTimeFormat.ShortTimePattern;
            }

            return shortPattern;
        }

        public static string FormatShortTime(DateTime dateTime, CultureInfo culture)
        {
            culture ??= CultureInfo.CurrentUICulture;
            var shortPattern = GetShortPattern();

            return dateTime.ToString(shortPattern, culture);
        }

        private static readonly Dictionary<string, string> s_cacheWithoutAMPM = new Dictionary<string, string>();

        public static string FormatShortTimeWithoutAMPM(DateTime dateTime, CultureInfo culture)
        {
            culture ??= CultureInfo.CurrentUICulture;
            var shortPattern = GetShortPattern();

            if (!s_cacheWithoutAMPM.TryGetValue(shortPattern, out var value))
            {
                s_cacheWithoutAMPM[shortPattern] = value = shortPattern.Replace("t", "");
            }

            return dateTime.ToString(value, culture);
        }

        private static readonly Dictionary<string, string> s_cacheAMPMOnly = new Dictionary<string, string>();

        public static string FormatShortTimeAMPM(DateTime dateTime, CultureInfo culture)
        {
            culture ??= CultureInfo.CurrentUICulture;
            var shortPattern = GetShortPattern();

            if (!shortPattern.Contains('t'))
            {
                return string.Empty;
            }

            if (!s_cacheAMPMOnly.TryGetValue(shortPattern, out var value))
            {
                s_cacheAMPMOnly[shortPattern] = value = new string(shortPattern.Where(c => c == 't').ToArray());
            }

            return dateTime.ToString(value, culture);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
            {
                return Format(dateTime, parameter as string, language);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
