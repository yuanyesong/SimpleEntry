using System;
using System.Windows.Data;

namespace SimpleEntry.Converter
{
    /// <summary>
    /// 取值范围是否可用
    /// 数据类型-取值范围，多值转换器
    /// 只有在数据类型可用及类型是数字和日期时才可用
    /// </summary>
    class ValueRangeIsEnabledConverter:IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0].ToString()=="True"&& (values[1].ToString()=="0"||values[1].ToString()=="2"))
            {
                    return true;
            }
            else
            {
                return false;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
