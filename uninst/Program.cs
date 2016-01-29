using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uninst
{
    class Program
    {
        [STAThread]

        static void Main(string[] args)
        {
            string sysroot = System.Environment.SystemDirectory;
            System.Diagnostics.Process.Start(sysroot + "//msiexec.exe", "/x {A9C2F1F2-1203-4A24-AC47-5A1F8194783D} /qr");
        }
    }
}
