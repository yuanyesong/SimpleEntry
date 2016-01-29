using System;
using System.Windows.Input;

namespace SimpleEntry.ViewModels.Windows
{
    /// <summary>
    /// 在忙碌时鼠标指针自动变成忙碌状态
    /// </summary>
    public class AutoWaitCursor : IDisposable
    {
        private Cursor _previousCursor;

        public AutoWaitCursor()
        {
            _previousCursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Mouse.OverrideCursor = _previousCursor;
        }
        #endregion
    }
}
