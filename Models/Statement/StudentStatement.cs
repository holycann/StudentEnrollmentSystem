using System.ComponentModel.DataAnnotations;
namespace StudentEnrollmentSystem.Models;

public class Transaction
{
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
}

public class StudentStatement
{
    public List<Transaction> Transactions { get; set; }
    public decimal TotalBalance { get; set; }
}