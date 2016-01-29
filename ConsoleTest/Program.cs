using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //string line = "    你好   ,\"   wome ma\"    ";
            //string[] parts = Regex.Split(line, "\\s*,\\s*");
            //Console.WriteLine(parts[0]);
            string s = "Hello 你好吗";
            byte[] sarr = System.Text.Encoding.Default.GetBytes(s);
            int len = sarr.Length;
            Console.WriteLine(len);
            Console.ReadKey();
        }
    }
}
