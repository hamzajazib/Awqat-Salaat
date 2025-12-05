using AwqatSalaat.Configurations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace AwqatSalaat.WinUI.Converters
{
    internal class ThemeAccentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ThemeAccent accent)
            {
                try
                {
                    var dict = (ResourceDictionary)App.Current.Resources[accent.ToString()];

                    var color = (Color)dict["SystemAccentColor"];

                    return new SolidColorBrush(color);
                }
                catch
                {
                    return new SolidColorBrush(Microsoft.UI.Colors.Magenta);
                }
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
