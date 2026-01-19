using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace AwqatSalaat.UI.Converters
{
    internal class MultiBooleanANDConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values?.Length > 0)
            {
                bool result = true;

                foreach (bool val in values.Cast<bool>())
                {
                    result = result && val;

                    if (!result)
                    {
                        break;
                    }
                }

                return result;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
