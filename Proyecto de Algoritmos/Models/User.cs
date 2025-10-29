using System;

namespace CajeroAutomatico.Models
{
    public class User
    {
        public string AccountNumber { get; set; } = "";
        public string Name { get; set; } = "";
        public string PinHash { get; set; } = ""; // Aca usamos el hash SHA-256 
    }
}
