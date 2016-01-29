using System;
using System.Windows.Data;

namespace SimpleEntry.Converter
{
    /// <summary>
    /// 题号重写ToString方法，例：第1题
    /// 题号-ToString，转换器
    /// </summary>
    class QuestionNumberToStringConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format("第{0}题",value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
