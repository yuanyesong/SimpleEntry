using System;

namespace SimpleEntry
{
    /// <summary>
    /// 修改后的程序入口点
    /// </summary>
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            SingleInstanceApp SIApp = new SingleInstanceApp();
            SIApp.Run(args);
        }
    }
}
