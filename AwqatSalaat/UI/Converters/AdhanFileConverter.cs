using AwqatSalaat.Data;
using AwqatSalaat.Helpers;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AwqatSalaat.UI.Converters
{
    internal class AdhanFileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AdhanSound adhanSound)
            {
                return AdhanConverter.AdhanSoundToFilePath(adhanSound, null, false);
            }
            else if (value is string filePath)
            {
                return AdhanConverter.FilePathToAdhanSound(filePath);
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AdhanSound adhanSound && targetType == typeof(string))
            {
                return adhanSound == AdhanSound.Custom
                    ? Binding.DoNothing // This allows the previously selected path to stay in the textbox
                    : AdhanConverter.AdhanSoundToFilePath(adhanSound, null, false);
            }
            else if (value is string filePath && targetType == typeof(AdhanSound))
            {
                return AdhanConverter.FilePathToAdhanSound(filePath);
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
