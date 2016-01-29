using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace SimpleEntry.ViewModels.Windows
{
    /// <summary>
    /// 按下Enter键，把用户输入的数字转换成日期格式，用于快速录入
    /// 例：20120305，2012/03/05
    /// </summary>
    public static class DatePickerHelper
    {
        #region EnableFastInput

        /// <summary>
        /// EnableFastInput Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableFastInputProperty =
            DependencyProperty.RegisterAttached("EnableFastInput", typeof(bool), typeof(DatePickerHelper),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnEnableFastInputChanged)));

        /// <summary>
        /// Gets the EnableFastInput property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static bool GetEnableFastInput(DependencyObject d)
        {
            return (bool)d.GetValue(EnableFastInputProperty);
        }

        /// <summary>
        /// Sets the EnableFastInput property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetEnableFastInput(DependencyObject d, bool value)
        {
            d.SetValue(EnableFastInputProperty, value);
        }

        /// <summary>
        /// Handles changes to the EnableFastInput property.
        /// </summary>
        private static void OnEnableFastInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var datePicker = d as DatePicker;
            if (datePicker != null)
            {
                if ((bool)e.NewValue)
                {
                    datePicker.DateValidationError += DatePickerOnDateValidationError;
                }
                else
                {
                    datePicker.DateValidationError -= DatePickerOnDateValidationError;
                }
            }
        }

        private static void DatePickerOnDateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            var datePicker = sender as DatePicker;
            if (datePicker != null)
            {
                var text = e.Text;
                DateTime dateTime;
                if (DateTime.TryParseExact(text, "yyyyMMdd", CultureInfo.CurrentUICulture, DateTimeStyles.None, out dateTime))
                {
                    datePicker.SelectedDate = dateTime;
                }
            }
        }

        #endregion
    }
}
