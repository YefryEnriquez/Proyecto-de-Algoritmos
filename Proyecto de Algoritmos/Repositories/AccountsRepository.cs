using System.Text.Json;
using AtmSim.Models;

namespace AtmSim.Repositories;

public class AccountsRepository
{
    private readonly string _filePath;
    public AccountsRepository(string dataDir)
    {
        Directory.CreateDirectory(dataDir);
        _filePath = Path.Combine(dataDir, "accounts.json");
        if (!File.Exists(_filePath))
            File.WriteAllText(_filePath, "[]");
    }

    public List<Account> GetAll()
    {
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Account>>(json) ?? new();
    }

    public Account? FindByAccount(string accountNumber)
    {
        return GetAll().FirstOrDefault(a => a.AccountNumber == accountNumber);
    }

    public void Upsert(Account account)
    {
        var all = GetAll();
        var existing = all.FindIndex(a => a.AccountNumber == account.AccountNumber);
        if (existing >= 0) all[existing] = account; else all.Add(account);
        Save(all);
    }

    private void Save(List<Account> accounts)
    {
        var json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}
