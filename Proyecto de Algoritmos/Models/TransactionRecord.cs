using System;

namespace SimuladorCajero.Models
{

}

public class TransactionRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string AccountNumber { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Type { get; set; } = "";
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string Details { get; set; } = "";
}