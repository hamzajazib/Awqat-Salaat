using AwqatSalaat.Data;
using AwqatSalaat.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwqatSalaat.WinUI.Converters
{
    internal class AdhanFileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
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

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is AdhanSound adhanSound && targetType == typeof(string))
            {
                return adhanSound == AdhanSound.Custom
                    ? DependencyProperty.UnsetValue // This allows the previously selected path to stay in the textbox
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
