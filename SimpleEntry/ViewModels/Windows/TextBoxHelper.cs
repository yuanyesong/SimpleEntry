using System.Windows;
using System.Windows.Controls.Primitives;

namespace SimpleEntry.ViewModels.Windows
{
    /// <summary>
    /// 在Textox激活后自动选中文字
    /// </summary>
    class TextBoxHelper
    {
            public static readonly DependencyProperty AutoSelectAllProperty =
                DependencyProperty.RegisterAttached("AutoSelectAll", typeof(bool), typeof(TextBoxHelper),
                    new FrameworkPropertyMetadata((bool)false,
                        new PropertyChangedCallback(OnAutoSelectAllChanged)));

            public static bool GetAutoSelectAll(TextBoxBase tb)
            {
                return (bool)tb.GetValue(AutoSelectAllProperty);
            }

            public static void SetAutoSelectAll(TextBoxBase tb, bool value)
            {
                tb.SetValue(AutoSelectAllProperty, value);
            }

            private static void OnAutoSelectAllChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var textBox = d as TextBoxBase;
                if (textBox != null)
                {
                    var flag = (bool)e.NewValue;
                    if (flag)
                    {
                        textBox.GotFocus += TextBoxOnGotFocus;
                    }
                    else
                    {
                        textBox.GotFocus -= TextBoxOnGotFocus;
                    }
                }
            }

            private static void TextBoxOnGotFocus(object sender, RoutedEventArgs e)
            {
                var textBox = sender as TextBoxBase;
                if (textBox != null)
                {
                    textBox.SelectAll();
                }
            }
        }
    }
