using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetProductCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var code=RegistryHelper.GetUpgradeCode(Guid.Parse("{F828D62B-51AC-49B7-902D-11DFA5E3F4E4}"));
            Console.WriteLine(code.ToString());
            Console.ReadKey();
        }
    }
}
