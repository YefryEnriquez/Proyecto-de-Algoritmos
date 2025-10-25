using System;
using SimuladorCajero.Persistence;
using SimuladorCajero.Utils;
using SimuladorCajero.Models;

namespace SimuladorCajero.Services
{

}

public class AuthService
{
    public bool ValidateCredentials(string accountNumber, string pin)
    {
        var acc = JsonDb.GetAccount(accountNumber);
        if (acc == null) return false;
        var hash = Hasher.Sha256(pin);
        return string.Equals(hash, acc.PinHash, StringComparison.OrdinalIgnoreCase);
    }

    public void Register(string accountNumber, string pin)
    {
        var existing = JsonDb.GetAccount(accountNumber);
        if (existing != null) throw new InvalidOperationException("El número de cuenta ya existe.");

        var acc = new Account
        {
            AccountNumber = accountNumber,
            PinHash = Hasher.Sha256(pin),
            Balance = 0m
        };
        JsonDb.UpsertAccount(acc);
    }
}
