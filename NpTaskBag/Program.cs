using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpTaskBag
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                new BagTaskTest().Action();
            }
            catch (Exception ex)
            {
                var temp = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ForegroundColor = temp;
            }
            Console.ReadLine();
        }
    }
}
