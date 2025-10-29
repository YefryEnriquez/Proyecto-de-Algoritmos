using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CajeroAutomatico.Models;

namespace CajeroAutomatico.Repositories
{
    public class TransactionsRepository
    {
        private readonly string _filePath;
        public TransactionsRepository(string dataDir)
        {
            Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, "transactions.json");
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        public List<TransactionRecord> GetAll()
        {
            var json = File.ReadAllText(_filePath);
            var result = JsonSerializer.Deserialize<List<TransactionRecord>>(json);
            return result != null ? result : new List<TransactionRecord>();
        }

        public List<TransactionRecord> GetByAccount(string accountNumber)
        {
            return GetAll().Where(t => t.AccountNumber == accountNumber).OrderByDescending(t => t.Timestamp).ToList();
        }

        public void Add(TransactionRecord record)
        {
            var all = GetAll();
            all.Add(record);
            Save(all);
        }

        private void Save(List<TransactionRecord> records)
        {
            var json = JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
