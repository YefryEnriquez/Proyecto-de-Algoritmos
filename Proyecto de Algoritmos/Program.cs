using System;
using System.Globalization;
using System.IO;



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

void LoginFlow()
{
    Console.Clear();
    Console.WriteLine("=== Inicio de Sesión ===");
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