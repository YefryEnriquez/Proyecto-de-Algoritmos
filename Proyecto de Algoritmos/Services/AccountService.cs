using System; // Agrega esta directiva using para que InvalidOperationException esté disponible
using SimuladorCajero.Persistence;
using SimuladorCajero.Models;

namespace SimuladorCajero.Services
{

}

public class AccountService
{
    public decimal GetBalance(string accountNumber)
    {
        var acc = JsonDb.GetAccount(accountNumber) ?? throw new InvalidOperationException("Cuenta no encontrada.");
        return acc.Balance;
    }

    public void Deposit(string accountNumber, decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("El monto debe ser mayor a cero.");
        var acc = JsonDb.GetAccount(accountNumber) ?? throw new InvalidOperationException("Cuenta no encontrada.");
        acc.Balance += amount;
        JsonDb.UpsertAccount(acc);

        JsonDb.AddTransaction(new TransactionRecord
        {
            AccountNumber = accountNumber,
            Amount = amount,
            BalanceAfter = acc.Balance,
            Type = "DEPÓSITO",
            Timestamp = DateTime.Now,
            Details = "Depósito en efectivo"
        });
    }

    public void Withdraw(string accountNumber, decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("El monto debe ser mayor a cero.");
        var acc = JsonDb.GetAccount(accountNumber) ?? throw new InvalidOperationException("Cuenta no encontrada.");
        if (acc.Balance < amount) throw new InvalidOperationException("Saldo insuficiente.");
        acc.Balance -= amount;
        JsonDb.UpsertAccount(acc);

        JsonDb.AddTransaction(new TransactionRecord
        {
            AccountNumber = accountNumber,
            Amount = amount,
            BalanceAfter = acc.Balance,
            Type = "RETIRO",
            Timestamp = DateTime.Now,
            Details = "Retiro en efectivo"
        });
    }

    public void Transfer(string fromAccount, string toAccount, decimal amount)
    {
        if (fromAccount == toAccount) throw new ArgumentException("La cuenta de destino no puede ser la misma.");
        if (amount <= 0) throw new ArgumentException("El monto debe ser mayor a cero.");

        var origen = JsonDb.GetAccount(fromAccount) ?? throw new InvalidOperationException("Cuenta origen no encontrada.");
        var destino = JsonDb.GetAccount(toAccount);
        if (destino == null) throw new InvalidOperationException("La cuenta destino no existe.");
        if (origen.Balance < amount) throw new InvalidOperationException("Saldo insuficiente.");

        origen.Balance -= amount;
        destino.Balance += amount;
        JsonDb.UpsertAccount(origen);
        JsonDb.UpsertAccount(destino);

        JsonDb.AddTransaction(new TransactionRecord
        {
            AccountNumber = fromAccount,
            Amount = amount,
            BalanceAfter = origen.Balance,
            Type = "TRANSFERENCIA ENVÍO",
            Timestamp = DateTime.Now,
            Details = $"A {toAccount}"
        });
        JsonDb.AddTransaction(new TransactionRecord
        {
            AccountNumber = toAccount,
            Amount = amount,
            BalanceAfter = destino.Balance,
            Type = "TRANSFERENCIA RECEPCIÓN",
            Timestamp = DateTime.Now,
            Details = $"Desde {fromAccount}"
        });
    }

    public static void CreateOpeningTransaction(string accountNumber, decimal initialDeposit, string details)
    {
        JsonDb.AddTransaction(new TransactionRecord
        {
            AccountNumber = accountNumber,
            Amount = initialDeposit,
            BalanceAfter = initialDeposit,
            Type = "APERTURA",
            Timestamp = DateTime.Now,
            Details = details
        });
    }
}
