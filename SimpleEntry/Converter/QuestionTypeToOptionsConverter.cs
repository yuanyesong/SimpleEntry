using System;
using System.Windows.Data;

namespace SimpleEntry.Converter
{
    /// <summary>
    /// 其他选项是否可用
    /// 题型-其他选项，转换器
    /// 只有选择题可用
    /// </summary>
    class QuestionTypeToOptionsConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value == 0 || (int)value == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
