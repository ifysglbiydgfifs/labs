using System;
using Rpn.Logic;

namespace Rpn
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите выражение: ");
            string input = Console.ReadLine();
            var rpn = new RpnCalculate(input);
            Console.WriteLine("Результат: " + rpn.Calculate());
        }
    }
}
