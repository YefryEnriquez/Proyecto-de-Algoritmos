using System.Globalization;
using AtmSim.Models;
using AtmSim.Repositories;

namespace AtmSim.Services;

public class AccountService
{
    private readonly AccountsRepository _accountsRepo;
    private readonly TransactionsRepository _txRepo;

    public AccountService(AccountsRepository accountsRepo, TransactionsRepository txRepo)
    {
        _accountsRepo = accountsRepo;
        _txRepo = txRepo;
    }

    public decimal GetBalance(string accountNumber)
    {
        var account = _accountsRepo.FindByAccount(accountNumber) ?? throw new InvalidOperationException("Cuenta no encontrada.");
        return account.Balance;
    }

    public void Deposit(string accountNumber, decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("El monto debe ser mayor a cero.");
        var account = _accountsRepo.FindByAccount(accountNumber) ?? throw new InvalidOperationException("Cuenta no encontrada.");
        account.Balance += amount;
        _accountsRepo.Upsert(account);

        _txRepo.Add(new TransactionRecord
        {
            AccountNumber = accountNumber,
            Amount = amount,
            BalanceAfter = account.Balance,
            Type = "DEPÓSITO",
            Timestamp = DateTime.Now,
            Details = "Depósito en efectivo."
        });
    }

    public void Withdraw(string accountNumber, decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("El monto debe ser mayor a cero.");
        var account = _accountsRepo.FindByAccount(accountNumber) ?? throw new InvalidOperationException("Cuenta no encontrada.");
        if (account.Balance < amount) throw new InvalidOperationException("Saldo insuficiente.");
        account.Balance -= amount;
        _accountsRepo.Upsert(account);

        _txRepo.Add(new TransactionRecord
        {
            AccountNumber = accountNumber,
            Amount = amount,
            BalanceAfter = account.Balance,
            Type = "RETIRO",
            Timestamp = DateTime.Now,
            Details = "Retiro en efectivo."
        });
    }

    public void Transfer(string fromAccount, string toAccount, decimal amount)
    {
        if (fromAccount == toAccount) throw new ArgumentException("La cuenta de destino no puede ser la misma.");
        if (amount <= 0) throw new ArgumentException("El monto debe ser mayor a cero.");

        var origen = _accountsRepo.FindByAccount(fromAccount) ?? throw new InvalidOperationException("Cuenta origen no encontrada.");
        var destino = _accountsRepo.FindByAccount(toAccount);
        if (destino is null) throw new InvalidOperationException("La cuenta destino no existe.");
        if (origen.Balance < amount) throw new InvalidOperationException("Saldo insuficiente.");

        origen.Balance -= amount;
        destino.Balance += amount;
        _accountsRepo.Upsert(origen);
        _accountsRepo.Upsert(destino);

        _txRepo.Add(new TransactionRecord
        {
            AccountNumber = fromAccount,
            Amount = amount,
            BalanceAfter = origen.Balance,
            Type = "TRANSFERENCIA ENVÍO",
            Timestamp = DateTime.Now,
            Details = $"Transferencia enviada a {toAccount}."
        });

        _txRepo.Add(new TransactionRecord
        {
            AccountNumber = toAccount,
            Amount = amount,
            BalanceAfter = destino.Balance,
            Type = "TRANSFERENCIA RECEPCIÓN",
            Timestamp = DateTime.Now,
            Details = $"Transferencia recibida desde {fromAccount}."
        });
    }

    public void CreateAccount(string accountNumber, string ownerName, decimal initialDeposit = 0m)
    {
        var existing = _accountsRepo.FindByAccount(accountNumber);
        if (existing is not null) throw new InvalidOperationException("El número de cuenta ya existe.");
        var account = new Account { AccountNumber = accountNumber, Balance = initialDeposit };
        _accountsRepo.Upsert(account);

        _txRepo.Add(new TransactionRecord
        {
            AccountNumber = accountNumber,
            Amount = initialDeposit,
            BalanceAfter = initialDeposit,
            Type = "APERTURA",
            Timestamp = DateTime.Now,
            Details = $"Cuenta creada para {ownerName}."
        });
    }

    public List<TransactionRecord> GetHistory(string accountNumber)
    {
        return _txRepo.GetByAccount(accountNumber);
    }
}
