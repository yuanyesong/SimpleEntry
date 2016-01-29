using Microsoft.VisualBasic.ApplicationServices;

namespace SimpleEntry
{
    /// <summary>
    /// 程序单实例运行
    /// </summary>
    public class SingleInstanceApp : WindowsFormsApplicationBase
    {
        App win = null;
        public SingleInstanceApp()
        {
            this.IsSingleInstance = true;
        }

        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs eventArgs)
        {
            win = new App();
            win.Run();
            return false;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            foreach (System.Windows.Window _win in win.Windows)
            {
                if (_win.Visibility == System.Windows.Visibility.Visible)
                {
                    _win.Activate();
                }
            }
        }
    }
}
