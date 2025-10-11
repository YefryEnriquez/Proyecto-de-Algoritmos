using System;
using System.Globalization;
using System.IO;

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("es-GT");
Console.OutputEncoding = System.Text.Encoding.UTF8;

var dataDir = Path.Combine(AppContext.BaseDirectory, "Data");

var usersRepo = new UsersRepository(dataDir);
var accountsRepo = new AccountsRepository(dataDir);
var txRepo = new TransactionsRepository(dataDir);

var auth = new AuthService(usersRepo);
var accountSvc = new AccountService(accountsRepo, txRepo);

// Seed si no hay datos
SeedIfEmpty();

while (true)
{
    Console.Clear();
    Console.WriteLine("========================================");
    Console.WriteLine("     SIMULADOR DE CAJERO AUTOMÁTICO     ");
    Console.WriteLine("========================================");
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