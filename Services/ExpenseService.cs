using Microsoft.EntityFrameworkCore;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.Models;

namespace PersonalExpenseTracker.Services;

public class ExpenseService
{
    private readonly AppDbContext _dbContext;

    public ExpenseService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Expense>> GetAllAsync()
    {
        return await _dbContext.Expenses
            .OrderByDescending(expense => expense.Date)
            .ToListAsync();
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        expense.CreatedAt = DateTime.UtcNow;

        _dbContext.Expenses.Add(expense);

        await _dbContext.SaveChangesAsync();

        return expense;
    }

    public async Task UpdateAsync(Expense expense)
    {
        var existingExpense = await _dbContext.Expenses.FindAsync(expense.Id);

        if (existingExpense is null)
        {
            throw new InvalidOperationException(
                $"Expense with ID {expense.Id} was not found.");
        }

        existingExpense.Title = expense.Title;
        existingExpense.Amount = expense.Amount;
        existingExpense.Category = expense.Category;
        existingExpense.Date = expense.Date;
        existingExpense.Notes = expense.Notes;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var expense = await _dbContext.Expenses.FindAsync(id);

        if (expense is null)
        {
            return;
        }

        _dbContext.Expenses.Remove(expense);

        await _dbContext.SaveChangesAsync();
    }
}
