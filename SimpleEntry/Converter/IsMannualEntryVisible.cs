using System;
using System.Windows.Data;

namespace SimpleEntry.Converter
{
    /// <summary>
    /// 录入时，手动输入答案框是否可见
    /// 题型-手动输入，转换器
    /// 填空题不可见
    /// </summary>
    class IsMannualEntryVisible:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value!=3)
            {
                return "Visible";
            }
            else
            {
                return "Collapsed";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
