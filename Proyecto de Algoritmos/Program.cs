using System;
using System.Globalization;
using System.IO;



class Program
{
    static AuthService auth = new AuthService();

    static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("     SIMULADOR DE CAJERO AUTOMÁTICO  \n   ");
            Console.WriteLine("1) Iniciar sesión");
            Console.WriteLine("2) Crear nueva cuenta");
            Console.WriteLine("3) Salir");
            Console.Write("Selecciona una opción: ");
            var op = Console.ReadLine();

            if (op == "1") LoginFlow();
            else if (op == "2") RegisterFlow();
            else if (op == "3") break;
            else ConsoleHelper.PressAnyKey("Opción inválida.");
        }
    }

    static void LoginFlow()
    {
        Console.Clear();
        Console.WriteLine(" Inicio de Sesión ");
        Console.Write("Número de cuenta: ");
        var account = (Console.ReadLine() ?? "").Trim();
        int intentos = 0;
        const int MAX_INTENTOS = 3;

        while (intentos < MAX_INTENTOS)
        {
            var pin = ConsoleHelper.ReadHidden("PIN: ");
            if (auth.ValidateCredentials(account, pin))
            {
                PostLoginMenu(account);
                return;
            }
            else
            {
                intentos++;
                Console.WriteLine($"Credenciales inválidas. Intentos restantes: {MAX_INTENTOS - intentos}");
                if (intentos >= MAX_INTENTOS)
                {
                    ConsoleHelper.PressAnyKey("Demasiados intentos. Regresando al menú principal...");
                    return;
                }
            }
        }
    }

    static void RegisterFlow()
    {
        Console.Clear();
        Console.WriteLine(" Crear Nueva Cuenta ");
        Console.Write("Número de cuenta (10 dígitos): ");
        var newAccount = (Console.ReadLine() ?? "").Trim();
        Console.Write("Define un PIN (4 dígitos): ");
        var pin = (Console.ReadLine() ?? "").Trim();

        if (newAccount.Length == 10 && pin.Length == 4)
        {
            Console.WriteLine("Cuenta creada con éxito.");
        }
        else
        {
            Console.WriteLine("Número de cuenta o PIN inválido.");
        }
        ConsoleHelper.PressAnyKey();
    }

    static void PostLoginMenu(string account)
    {
        
    }
}

class ConsoleHelper
{
    public static void PressAnyKey(string message = null)
    {
        Console.WriteLine(message ?? "Presiona cualquier tecla para continuar...");
        Console.ReadKey();
    }

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
}

class AuthService
{
    public bool ValidateCredentials(string accountNumber, string pin)
    {
        return accountNumber == "1000000001" && pin == "1234";
    }
}




