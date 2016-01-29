using System;
using System.Windows.Data;

namespace SimpleEntry.Converter
{
    /// <summary>
    /// 完成按钮是否可见
    /// TabName-完成，转换器
    /// 只有建库选项卡完成按钮才可见
    /// </summary>
    class IsFinishButtonVisible:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string tabName = "建库";
            if ((string)value== tabName)
            {
                return "visible";
            }
            else
            {
                return "collapsed";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
