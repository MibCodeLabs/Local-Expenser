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

    public async Task<List<Expense>> GetAllAsync(
        string? search = null,
        string? category = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        bool sortAscending = false
    )
    {
        IQueryable<Expense> query = _dbContext.Expenses;

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(expense =>
                expense.Title.Contains(search)
                || (expense.Notes != null && expense.Notes.Contains(search))
            );
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(expense => expense.Category == category);
        }

        if (startDate.HasValue)
        {
            query = query.Where(expense => expense.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(expense => expense.Date <= endDate.Value);
        }

        query = sortAscending
            ? query.OrderBy(expense => expense.Date)
            : query.OrderByDescending(expense => expense.Date);

        return await query.ToListAsync();
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
            throw new InvalidOperationException($"Expense with ID {expense.Id} was not found.");
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

    public async Task<decimal> GetTotalSpendingAsync()
    {
        var total = await _dbContext.Expenses.SumAsync(expense => expense.Amount);

        return Math.Round(total, 3);
    }

    public async Task<decimal> GetCurrentMonthSpendingAsync()
    {
        var now = DateTime.UtcNow;

        var startOfMonth = new DateTime(now.Year, now.Month, 1);

        var startOfNextMonth = startOfMonth.AddMonths(1);

        var total = await _dbContext
            .Expenses.Where(expense =>
                expense.Date >= startOfMonth && expense.Date < startOfNextMonth
            )
            .SumAsync(expense => expense.Amount);

        return Math.Round(total, 3);
    }

    public async Task<int> GetExpenseCountAsync()
    {
        return await _dbContext.Expenses.CountAsync();
    }

    public async Task<Dictionary<string, decimal>> GetSpendingByCategoryAsync()
    {
        return await _dbContext
            .Expenses.GroupBy(expense => expense.Category)
            .Select(group => new
            {
                Category = group.Key,
                Total = Math.Round(group.Sum(expense => expense.Amount), 3)
            })
            .ToDictionaryAsync(item => item.Category, item => item.Total);
    }

    public async Task<List<Expense>> GetRecentExpensesAsync(int count = 5)
    {
        return await _dbContext
            .Expenses.OrderByDescending(expense => expense.Date)
            .Take(count)
            .ToListAsync();
    }
}
