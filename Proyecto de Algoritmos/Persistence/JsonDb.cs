using System.Text.Json;
using System.IO; // <-- Agregar esta línea para Path y File
using System;    // <-- Agregar esta línea para AppContext
using System.Linq;
using System.Collections.Generic;
using SimuladorCajero.Models;
using SimuladorCajero.Utils; // <-- Agregar using SimuladorCajero.Utils;

namespace SimuladorCajero.Persistence
{

}

public static class JsonDb
{
    public static readonly string DataDir = Path.Combine(AppContext.BaseDirectory, "Data");
    public static readonly string AccountsFile = Path.Combine(DataDir, "accounts.json");
    public static readonly string TransactionsFile = Path.Combine(DataDir, "transactions.json");

    public static void EnsureFiles()
    {
        Directory.CreateDirectory(DataDir);
        if (!File.Exists(AccountsFile)) File.WriteAllText(AccountsFile, "[]");
        if (!File.Exists(TransactionsFile)) File.WriteAllText(TransactionsFile, "[]");

        var accs = LoadAccounts();
        if (accs.Count == 0)
        {
            var demo = new Account { AccountNumber = "1000000001", PinHash = SimuladorCajero.Utils.Hasher.Sha256("1234"), Balance = 1000m };
            accs.Add(demo);
            SaveAccounts(accs);
            AddTransaction(new TransactionRecord
            {
                AccountNumber = demo.AccountNumber,
                Amount = demo.Balance,
                BalanceAfter = demo.Balance,
                Type = "APERTURA",
                Timestamp = DateTime.Now,
                Details = "Cuenta demo creada"
            });
        }
    }

    public static List<Account> LoadAccounts()
    {
        var json = File.ReadAllText(AccountsFile);
        var result = JsonSerializer.Deserialize<List<Account>>(json);
        return result != null ? result : new List<Account>();
    }

    public static void SaveAccounts(List<Account> accounts)
    {
        var json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(AccountsFile, json);
    }

    public static Account GetAccount(string accountNumber)
    {
        return LoadAccounts().FirstOrDefault(a => a.AccountNumber == accountNumber);
    }

    public static void UpsertAccount(Account account)
    {
        var all = LoadAccounts();
        var idx = all.FindIndex(a => a.AccountNumber == account.AccountNumber);
        if (idx >= 0) all[idx] = account; else all.Add(account);
        SaveAccounts(all);
    }

    public static List<TransactionRecord> LoadTransactions()
    {
        var json = File.ReadAllText(TransactionsFile);
        var result = JsonSerializer.Deserialize<List<TransactionRecord>>(json);
        return result != null ? result : new List<TransactionRecord>();
    }

    public static void SaveTransactions(List<TransactionRecord> txs)
    {
        var json = JsonSerializer.Serialize(txs, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(TransactionsFile, json);
    }

    public static void AddTransaction(TransactionRecord record)
    {
        var all = LoadTransactions();
        all.Add(record);
        SaveTransactions(all);
    }

    public static List<TransactionRecord> GetTransactionsFor(string account)
    {
        return LoadTransactions().Where(t => t.AccountNumber == account).OrderByDescending(t => t.Timestamp).ToList();
    }
}
