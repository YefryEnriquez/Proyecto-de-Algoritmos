namespace SimuladorCajero.Models
{

}

public class Account
{
    public string AccountNumber { get; set; } = "";
    public string PinHash { get; set; } = "";
    public decimal Balance { get; set; } = 0m;
}
