using System;
using System.Windows.Data;

namespace SimpleEntry.Converter
{
    /// <summary>
    /// 值标签是否可用
    /// 题型-值标签，转换器
    /// 单选题才可用
    /// </summary>
    class QuestionTypeToValueLableConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((int)value == 0)
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
            throw new NotImplementedException();
        }
    }
}
