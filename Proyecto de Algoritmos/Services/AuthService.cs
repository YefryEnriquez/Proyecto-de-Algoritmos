using System; // Agrega esta línea para StringComparison
using CajeroAutomatico.Models;
using CajeroAutomatico.Repositories;
using CajeroAutomatico.Utils;

    namespace CajeroAutomatico.Services
    {
        public class AuthService
        {
            private readonly UsersRepository _usersRepo;

            public AuthService(UsersRepository usersRepo)
            {
                _usersRepo = usersRepo;
            }

            public bool ValidateCredentials(string accountNumber, string pinPlain)
            {
                var user = _usersRepo.FindByAccount(accountNumber);
                if (user is null) return false;
                var pinHash = Hasher.Sha256(pinPlain);
                return string.Equals(pinHash, user.PinHash, StringComparison.OrdinalIgnoreCase);
            }

            public void Register(string accountNumber, string name, string pinPlain)
            {
                var user = new User
                {
                    AccountNumber = accountNumber,
                    Name = name,
                    PinHash = Hasher.Sha256(pinPlain)
                };
                _usersRepo.Upsert(user);
            }
        }
    }
