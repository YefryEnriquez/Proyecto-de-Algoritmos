using System;
using System.Globalization;
using System.Linq;
using System.Text;
using SimuladorCajero.Persistence;
using SimuladorCajero.Services;
using SimuladorCajero.Utils;

namespace SimuladorCajero
{
    class Program
    {
        static AuthService auth = new AuthService();
        static AccountService accountSvc = new AccountService();

        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("es-GT");
            Console.OutputEncoding = Encoding.UTF8;

            JsonDb.EnsureFiles();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("     SIMULADOR CAJERO  \n");
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
            Console.Write("Define un PIN (4-6 dígitos): ");
            var pin = (Console.ReadLine() ?? "").Trim();

            if (newAccount.Length == 10 && newAccount.All(char.IsDigit) &&
                pin.Length >= 4 && pin.Length <= 6 && pin.All(char.IsDigit))
            {
                if (JsonDb.GetAccount(newAccount) != null)
                {
                    Console.WriteLine("Ese número de cuenta ya existe.");
                }
                else
                {
                    auth.Register(newAccount, pin);
                    AccountService.CreateOpeningTransaction(newAccount, 0m, "Nueva cuenta");
                    Console.WriteLine("Cuenta creada con éxito.");
                }
            }
            else
            {
                Console.WriteLine("Número de cuenta o PIN inválido.");
            }
            ConsoleHelper.PressAnyKey();
        }

        static void PostLoginMenu(string account)
        {
            while (true)
            {
                Console.Clear();
                var balance = accountSvc.GetBalance(account);
                Console.WriteLine($" Menú Principal (Cuenta {account}) ");
                Console.WriteLine($"Saldo actual: {balance.ToString("C")}");
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
                            Console.WriteLine($"Tu saldo es: {accountSvc.GetBalance(account).ToString("C")}");
                            ConsoleHelper.PressAnyKey();
                            break;
                        case "2":
                            {
                                var amount = ConsoleHelper.ReadPositiveAmount("Monto a retirar: ");
                                accountSvc.Withdraw(account, amount);
                                Console.WriteLine("Retiro realizado con éxito.");
                                ConsoleHelper.PressAnyKey();
                                break;
                            }
                        case "3":
                            {
                                var amount = ConsoleHelper.ReadPositiveAmount("Monto a depositar: ");
                                accountSvc.Deposit(account, amount);
                                Console.WriteLine("Depósito realizado con éxito.");
                                ConsoleHelper.PressAnyKey();
                                break;
                            }
                        case "4":
                            {
                                Console.Write("Cuenta destino (10 dígitos): ");
                                var dest = (Console.ReadLine() ?? "").Trim();
                                if (string.IsNullOrWhiteSpace(dest) || dest.Length != 10 || !dest.All(char.IsDigit))
                                {
                                    Console.WriteLine("Cuenta destino inválida.");
                                    ConsoleHelper.PressAnyKey();
                                    break;
                                }
                                var amount = ConsoleHelper.ReadPositiveAmount("Monto a transferir: ");
                                accountSvc.Transfer(account, dest, amount);
                                Console.WriteLine("Transferencia realizada con éxito.");
                                ConsoleHelper.PressAnyKey();
                                break;
                            }
                        case "5":
                            {
                                var hist = JsonDb.GetTransactionsFor(account);
                                if (hist.Count == 0)
                                {
                                    Console.WriteLine("No hay transacciones registradas.");
                                }
                                else
                                {
                                    Console.WriteLine("Fecha/Hora              Tipo                         Monto        Saldo       Detalles");
                                    Console.WriteLine(new string('-', 100));
                                    foreach (var t in hist.OrderByDescending(h => h.Timestamp))
                                    {
                                        Console.WriteLine($"{t.Timestamp:yyyy-MM-dd HH:mm} | {t.Type,-26} | {t.Amount,10:C} | {t.BalanceAfter,10:C} | {t.Details}");
                                    }
                                }
                                ConsoleHelper.PressAnyKey();
                                break;
                            }
                        case "6":
                            return;
                        default:
                            Console.WriteLine("Opción inválida.");
                            ConsoleHelper.PressAnyKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ConsoleHelper.PressAnyKey();
                }
            }
        }
    }
}