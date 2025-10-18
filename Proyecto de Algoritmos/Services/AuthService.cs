using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimuladorCajero.Repositories;
using SimuladorCajero.Models;


namespace SimuladorCajero.Services
{
    public class AuthService
    {
        private readonly AccountsRepository _accountsRepo;

        public AuthService(AccountsRepository accountsRepo)
        {
            _accountsRepo = accountsRepo;
        }

        public bool ValidateCredentials(string accountNumber, string pin)
        {
            var account = _accountsRepo.FindByAccount(accountNumber);
            if (account == null)
                return false;

            // Compara el PIN (en un caso real, usaría un hash SHA-256)
            return pin == "1234"; // Solo para demo
        }

        public void Register(string accountNumber, string pin)
        {
            var account = new Account
            {
                AccountNumber = accountNumber,

                PinHash = pin // Aquí usaríamos un hash en un caso real
            };
            _accountsRepo.Upsert(account);
        }
    }
}
