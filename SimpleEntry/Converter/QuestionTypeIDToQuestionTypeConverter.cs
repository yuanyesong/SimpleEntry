using System;
using System.Windows.Data;

namespace SimpleEntry.Converter
{
    /// <summary>
    /// 题型编号-题型转换器，例：0-单选题
    /// </summary>
    class QuestionTypeIDToQuestionTypeConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value.ToString())
            {
                case "0":
                    return "单选题";
                case "1":
                    return "多选题";
                case "2":
                    return "判断题";
                case "3":
                    return "填空题";
                default:
                    return "未知";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
