using System;
using System.Windows;

namespace SimpleEntry.ViewModels.Windows
{
    /// <summary>
    /// 使MessageBox在窗口中居中显示
    /// </summary>
    public class MessageBoxPlus
    {
        static System.Threading.Timer _timeoutTimer;
        static string _caption;
        public static MessageBoxResult Show(Window owner, String messageBoxText)
        {
            if (owner.Dispatcher.CheckAccess())
            {
                owner.CenterChild();
                return MessageBox.Show(owner, messageBoxText);
            }
            else
            {
                return (MessageBoxResult)owner.Dispatcher.Invoke(new Func<MessageBoxResult>(() =>
                {
                    owner.CenterChild();
                    return MessageBox.Show(owner, messageBoxText);
                }));
            }
        }

        public static MessageBoxResult Show(Window owner, String messageBoxText, String caption)
        {
            if (owner.Dispatcher.CheckAccess())
            {
                owner.CenterChild();
                return MessageBox.Show(owner, messageBoxText, caption);
            }
            else
            {
                return (MessageBoxResult)owner.Dispatcher.Invoke(new Func<MessageBoxResult>(() =>
                {
                    owner.CenterChild();
                    return MessageBox.Show(owner, messageBoxText, caption);
                }));
            }
        }

        public static MessageBoxResult Show(Window owner, String messageBoxText, String caption, MessageBoxButton button)
        {
            if (owner.Dispatcher.CheckAccess())
            {
                owner.CenterChild();
                return MessageBox.Show(owner, messageBoxText, caption, button);
            }
            else
            {
                return (MessageBoxResult)owner.Dispatcher.Invoke(new Func<MessageBoxResult>(() =>
                {
                    owner.CenterChild();
                    return MessageBox.Show(owner, messageBoxText, caption, button);
                }));
            }
        }

        public static MessageBoxResult Show(Window owner, String messageBoxText, String caption, MessageBoxButton button, MessageBoxImage icon)
        {
            if (owner.Dispatcher.CheckAccess())
            {
                owner.CenterChild();
                return MessageBox.Show(owner, messageBoxText, caption, button, icon);
            }
            else
            {
                return (MessageBoxResult)owner.Dispatcher.Invoke(new Func<MessageBoxResult>(() =>
                {
                    owner.CenterChild();
                    return MessageBox.Show(owner, messageBoxText, caption, button, icon);
                }));
            }
        }

        public static MessageBoxResult Show(Window owner, String messageBoxText, String caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            if (owner.Dispatcher.CheckAccess())
            {
                owner.CenterChild();
                return MessageBox.Show(owner, messageBoxText, caption, button, icon, defaultResult);
            }
            else
            {
                return (MessageBoxResult)owner.Dispatcher.Invoke(new Func<MessageBoxResult>(() =>
                {
                    owner.CenterChild();
                    return MessageBox.Show(owner, messageBoxText, caption, button, icon, defaultResult);
                }));
            }
        }

        public static MessageBoxResult Show(Window owner, String messageBoxText, String caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
        {
            if (owner.Dispatcher.CheckAccess())
            {
                owner.CenterChild();
                return MessageBox.Show(owner, messageBoxText, caption, button, icon, defaultResult, options);
            }
            else
            {
                return (MessageBoxResult)owner.Dispatcher.Invoke(new Func<MessageBoxResult>(() =>
                {
                    owner.CenterChild();
                    return MessageBox.Show(owner, messageBoxText, caption, button, icon, defaultResult, options);
                }));
            }
        }

        /// <summary>
        /// 设置时间自动关闭MessageBox
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="messageBoxText"></param>
        /// <param name="caption"></param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static MessageBoxResult Show(Window owner, String messageBoxText, String caption, MessageBoxButton button, MessageBoxImage icon,int timeout)
        {
            if (owner.Dispatcher.CheckAccess())
            {
                owner.CenterChild();
                _caption = caption;
                _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
           null, timeout, System.Threading.Timeout.Infinite);
                return MessageBox.Show(owner, messageBoxText, caption, button, icon);
            }
            else
            {
                return (MessageBoxResult)owner.Dispatcher.Invoke(new Func<MessageBoxResult>(() =>
                {
                    owner.CenterChild();
                    _caption = caption;
                    _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
               null, timeout, System.Threading.Timeout.Infinite);
                    return MessageBox.Show(owner, messageBoxText, caption, button, icon);
                }));
            }
        }

        private static void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow(null, _caption);
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
        }
        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }
}
