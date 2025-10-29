using System.Collections.Generic;
using CajeroAutomatico.Models;
using CajeroAutomatico.Repositories;

namespace CajeroAutomatico.Services
{
    public class TransactionService
    {
        private readonly TransactionsRepository _repo;

        public TransactionService(TransactionsRepository repo)
        {
            _repo = repo;
        }

        public List<TransactionRecord> GetAllFor(string account) => _repo.GetByAccount(account);
    }
}
