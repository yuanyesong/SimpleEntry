using System;
using System.Collections;
using System.Windows.Data;

namespace SimpleEntry.Converter
{
    class BuildFormValidation:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool hasError = ((IList)values).Contains(true);
            if (hasError)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
