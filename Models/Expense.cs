using System.ComponentModel.DataAnnotations;

namespace PersonalExpenseTracker.Models;

public class Expense
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "99999999")]
    public decimal Amount { get; set; }

    [Required]
    public string Category { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}
