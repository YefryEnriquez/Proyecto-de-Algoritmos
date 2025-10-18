using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorCajero.Utils
{
    public static class ConsoleHelper
    {
        public static string ReadHidden(string prompt)
        {
            Console.Write(prompt);
            var pwd = new System.Text.StringBuilder();
            ConsoleKey key;
            while ((key = Console.ReadKey(true).Key) != ConsoleKey.Enter)
            {
                if (key == ConsoleKey.Backspace && pwd.Length > 0)
                {
                    pwd.Length--;
                    Console.Write("\\b \\b");
                }
                else if (!char.IsControl((char)key))
                {
                    pwd.Append((char)key);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return pwd.ToString();
        }

        public static void PressAnyKey(string message = null)
        {
            Console.WriteLine(message ?? "Presiona cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}