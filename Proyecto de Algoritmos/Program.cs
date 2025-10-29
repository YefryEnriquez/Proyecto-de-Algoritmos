using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CajeroAutomatico.Repositories;
using CajeroAutomatico.Services;
using CajeroAutomatico.Utils;

namespace ProyectoDeAlgoritmos
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("es-GT");
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var dataDir = Path.Combine(AppContext.BaseDirectory, "Data");

            var usersRepo = new UsersRepository(dataDir);
            var accountsRepo = new AccountsRepository(dataDir);
            var txRepo = new TransactionsRepository(dataDir);

            var auth = new AuthService(usersRepo);
            var accountSvc = new AccountService(accountsRepo, txRepo);


            SeedIfEmpty();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("     SIMULADOR DE CAJERO AUTOMÁTICO \n    ");
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

            void RegisterFlow()
            {
                Console.Clear();
                Console.WriteLine("=== Crear Nueva Cuenta ===");
                Console.Write("Nombre del titular: ");
                var name = Console.ReadLine() ?? "Usuario";
                string newAccount = GenerateAccountNumber();
                Console.WriteLine($"Número de cuenta sugerido: {newAccount}");
                Console.Write("Presiona Enter para aceptar o escribe otro número (10 dígitos): ");
                var custom = (Console.ReadLine() ?? "").Trim();
                if (!string.IsNullOrWhiteSpace(custom))
                {
                    if (custom.Length != 10 || !custom.All(char.IsDigit))
                    {
                        ConsoleHelper.PressAnyKey("Número inválido. Usando el sugerido.");
                    }
                    else
                    {
                        newAccount = custom;
                    }
                }

                if (accountsRepo.FindByAccount(newAccount) != null)
                {
                    ConsoleHelper.PressAnyKey("Ese número de cuenta ya existe. Intenta de nuevo.");
                    return;
                }

                string pin1 = ConsoleHelper.ReadHidden("Define un PIN (4-6 dígitos): ");
                string pin2 = ConsoleHelper.ReadHidden("Confirma el PIN: ");
                if (pin1 != pin2 || pin1.Length < 4 || pin1.Length > 6 || !pin1.All(char.IsDigit))
                {
                    ConsoleHelper.PressAnyKey("PIN inválido o no coincide. Operación cancelada.");
                    return;
                }

                decimal initial = 0m;
                Console.Write("Depósito inicial (opcional, Enter para 0): ");
                var s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s) && decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out var val) && val > 0)
                    initial = decimal.Round(val, 2);

                auth.Register(newAccount, name, pin1);
                accountSvc.CreateAccount(newAccount, name, initial);

                Console.WriteLine($"Cuenta creada con éxito para {name}. Número: {newAccount}");
                ConsoleHelper.PressAnyKey("Presiona cualquier tecla para continuar...");
            }

            void PostLoginMenu(string account)
            {
                while (true)
                {
                    Console.Clear();
                    var balance = accountSvc.GetBalance(account);
                    Console.WriteLine($"=== Menú Principal (Cuenta {account}) ===");
                    Console.WriteLine($"Saldo actual: {balance:C}"); // IDE0071: Simplificar interpolación
                    Console.WriteLine("1) Consultar saldo");
                    Console.WriteLine("2) Retirar dinero");
                    Console.WriteLine("3) Depositar dinero");
                    Console.WriteLine("4) Transferir a otra cuenta");
                    Console.WriteLine("5) Ver historial de transacciones");
                    Console.WriteLine("6) Cerrar sesión");
                    Console.Write("Selecciona una opción: ");
                    var op = Console.ReadLine();

                    try
                    {
                        switch (op)
                        {
                            case "1":
                                Console.WriteLine($"Tu saldo es: {accountSvc.GetBalance(account):C}"); // IDE0071
                                ConsoleHelper.PressAnyKey("Presiona cualquier tecla para continuar...");
                                break;
                            case "2":
                                {
                                    var amount = ConsoleHelper.ReadPositiveAmount("Monto a retirar: ");
                                    accountSvc.Withdraw(account, amount);
                                    Console.WriteLine("Retiro realizado con éxito.");
                                    ConsoleHelper.PressAnyKey("Presiona cualquier tecla para continuar...");
                                    break;
                                }
                            case "3":
                                {
                                    var amount = ConsoleHelper.ReadPositiveAmount("Monto a depositar: ");
                                    accountSvc.Deposit(account, amount);
                                    Console.WriteLine("Depósito realizado con éxito.");
                                    ConsoleHelper.PressAnyKey("Presiona cualquier tecla para continuar...");
                                    break;
                                }
                            case "4":
                                {
                                    Console.Write("Cuenta destino (10 dígitos): ");
                                    var dest = (Console.ReadLine() ?? "").Trim();
                                    if (string.IsNullOrWhiteSpace(dest) || dest.Length != 10 || !dest.All(char.IsDigit))
                                    {
                                        Console.WriteLine("Cuenta destino inválida.");
                                        ConsoleHelper.PressAnyKey("Presiona cualquier tecla para continuar...");
                                        break;
                                    }
                                    var amount = ConsoleHelper.ReadPositiveAmount("Monto a transferir: ");
                                    accountSvc.Transfer(account, dest, amount);
                                    Console.WriteLine("Transferencia realizada con éxito.");
                                    ConsoleHelper.PressAnyKey("Presiona cualquier tecla para continuar...");
                                    break;
                                }
                            case "5":
                                {
                                    var hist = accountSvc.GetHistory(account);
                                    if (hist.Count == 0)
                                    {
                                        Console.WriteLine("No hay transacciones registradas.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("Fecha/Hora                 Tipo                        Monto        Saldo       Detalles");
                                        Console.WriteLine(new string('-', 95));
                                        foreach (var t in hist.OrderByDescending(h => h.Timestamp))
                                        {
                                            Console.WriteLine($"{t.Timestamp:yyyy-MM-dd HH:mm} | {t.Type,-24} | {t.Amount,10:C} | {t.BalanceAfter,10:C} | {t.Details}");
                                        }
                                    }
                                    ConsoleHelper.PressAnyKey("Presiona cualquier tecla para continuar...");
                                    break;
                                }
                            case "6":
                                return;
                            default:
                                Console.WriteLine("Opción inválida.");
                                ConsoleHelper.PressAnyKey("Presiona cualquier tecla para continuar...");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        ConsoleHelper.PressAnyKey("Presiona cualquier tecla para continuar...");
                    }
                }
            }

            void SeedIfEmpty()
            {
                var users = usersRepo.GetAll();
                var accounts = accountsRepo.GetAll();
                if (users.Count == 0 && accounts.Count == 0)
                {
                    var demoAccount = "1000000001";
                    var demoName = "Usuario Demo";
                    var demoPin = "1234";
                    auth.Register(demoAccount, demoName, demoPin);
                    accountSvc.CreateAccount(demoAccount, demoName, 1000m);
                    Console.WriteLine("Se crearon datos de ejemplo. Cuenta: 1000000001, PIN: 1234 (solo demostración).");
                }
            }

            string GenerateAccountNumber()
            {
                // 10 dígitos, iniciando con 1
                var rng = new Random();
                string num;
                do
                {
                    num = "1" + string.Concat(Enumerable.Range(0, 9).Select(_ => rng.Next(0, 10).ToString()));
                } while (accountsRepo.FindByAccount(num) != null);
                return num;
            }
        }
    }
}