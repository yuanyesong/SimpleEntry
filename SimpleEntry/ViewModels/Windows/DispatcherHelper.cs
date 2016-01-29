using SimpleEntry.Services;
using System;
using System.Security.Permissions;
using System.Windows.Threading;

namespace SimpleEntry.ViewModels.Windows
{
    /// <summary>
    /// 状态栏更新文字不会被覆盖
    /// DoEvents空方法
    /// </summary>
    static class DispatcherHelper
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), frame);
            try { Dispatcher.PushFrame(frame); }
            catch (InvalidOperationException e) 
            {
                MessageBoxPlus.Show(App.Current.MainWindow, e.Message, "错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                LogWritter.Log(e,"DispatcherHelper发生错误");
                return;
            }
        }
        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }   
    }
}
