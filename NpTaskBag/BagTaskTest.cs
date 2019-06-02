using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpTaskBag
{
    public class BagTaskTest
    {
        public void Action()
        {
            Console.WriteLine("Start Test");
            int procElement = 10;

            var b = new BagTask<int>();

            b.ReadArray();

            //int countElement = b.AllElements.Count * procElement / 100;
            //var array = b.RandomArray(countElement);
            //Console.WriteLine($"Количество элементов - {countElement}");
            //var values = array.Select(c => b.AllElements[c]).ToList();
            b.MaxSize = 85782446;// array.Sum();
            Console.WriteLine($"Попытаемся найти число - {b.MaxSize}");
            b.Action();
        }
    }
}
