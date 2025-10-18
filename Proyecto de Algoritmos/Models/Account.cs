namespace SimuladorCajero.Models
{
    public class Account
    {
        public string AccountNumber { get; set; } = "";
        public string PinHash { get; set; } = ""; // Aca se aplica el hash del PIN
    }
}