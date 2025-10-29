using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CajeroAutomatico.Models;

namespace CajeroAutomatico.Repositories
{
    public class UsersRepository
    {
        private readonly string _filePath;
        public UsersRepository(string dataDir)
        {
            Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, "users.json");
            if (!File.Exists(_filePath))
                File.WriteAllText(_filePath, "[]");
        }

        public List<User> GetAll()
        {
            var json = File.ReadAllText(_filePath);
            var users = JsonSerializer.Deserialize<List<User>>(json);
            return users != null ? users : new List<User>();
        }

        public User FindByAccount(string accountNumber)
        {
            var allUsers = GetAll();
            foreach (var u in allUsers)
            {
                if (u.AccountNumber == accountNumber)
                    return u;
            }
            return null;
        }

        public void Upsert(User user)
        {
            var all = GetAll();
            var existing = all.FindIndex(u => u.AccountNumber == user.AccountNumber);
            if (existing >= 0)
                all[existing] = user;
            else
                all.Add(user);
            Save(all);
        }

        private void Save(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
