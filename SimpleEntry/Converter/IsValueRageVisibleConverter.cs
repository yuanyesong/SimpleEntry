using System;
using System.Windows.Data;

namespace SimpleEntry.Converter
{
    /// <summary>
    /// 录入时，取直范围是否可见
    /// 取值范围-可见，转换器
    /// 取值范围不为空时可见
    /// </summary>
    class IsValueRageVisibleConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrEmpty((string)value))
            {
                return "Collapsed";
            }
            else
            {
                return "Visible";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
