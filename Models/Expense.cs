namespace PersonalExpenseTracker.Models;

public class Expense
{
    public int Id {get; set;}
    public string Title {get; set;}="";
    
    public decimal Amount { get; set; }

    public string Category { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } 
}
