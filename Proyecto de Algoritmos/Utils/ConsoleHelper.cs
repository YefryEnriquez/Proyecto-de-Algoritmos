using System;
using System.Globalization;

namespace CajeroAutomatico.Utils
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
                    Console.Write("\b \b");
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

        public static decimal ReadPositiveAmount(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var s = Console.ReadLine();
                if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.GetCultureInfo("es-GT"), out var amount) && amount > 0)
                    return decimal.Round(amount, 2);
                Console.WriteLine("Monto inválido. Intenta de nuevo (usa punto o coma según tu configuración).");
            }
        }

        public static void PressAnyKey(string message)
        {
            Console.WriteLine(message ?? "Presiona cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}
