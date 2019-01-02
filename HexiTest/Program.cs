using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace HexiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Counter c = new Counter();
            c.On10 = On10;
            while (true)
            {
                c.Next();
            }
            Console.ReadKey();
        }

        static void On10 (int value)
        {
            Console.WriteLine("10的倍数");
        }

    }

    class Counter
    {
        public OnCountDelegate On10;

        private int i = 0;
        public void Next ()
        {
            i++;
            if (i % 10 == 0)
            {
                if(On10 != null)
                {
                    On10(i);
                }
            }
        }
    }


    delegate void OnCountDelegate(int value);
    
}
