using System;
using System.Globalization;

namespace SimuladorCajero.Utils
{

}

public static class ConsoleHelper
{
    public static void PressAnyKey(string message = null)
    {
        Console.WriteLine(message ?? "Presiona cualquier tecla para continuar...");
        Console.ReadKey();
    }

    public static string ReadHidden(string prompt)
    {
        Console.Write(prompt);
        var sb = new System.Text.StringBuilder();
        while (true)
        {
            var keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Enter) break;

            if (keyInfo.Key == ConsoleKey.Backspace && sb.Length > 0)
            {
                sb.Length--;
                Console.Write("\b \b");
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                sb.Append(keyInfo.KeyChar);
                Console.Write("*");
            }
        }
        Console.WriteLine();
        return sb.ToString();
    }

    public static decimal ReadPositiveAmount(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.GetCultureInfo("es-GT"), out var amount) && amount > 0)
                return decimal.Round(amount, 2);
            Console.WriteLine("Monto inválido. Intenta de nuevo.");
        }
    }
}
