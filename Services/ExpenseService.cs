using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using PersonalExpenseTracker.Data;
using PersonalExpenseTracker.Models;

namespace PersonalExpenseTracker.Services;

public class ExpenseService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ExpenseService> _logger;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public ExpenseService(
        AppDbContext dbContext,
        ILogger<ExpenseService> logger,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _dbContext = dbContext;
        _logger = logger;
        _authenticationStateProvider = authenticationStateProvider;
    }

    private async Task<string> GetCurrentUserIdAsync()
    {
        var authenticationState =
            await _authenticationStateProvider.GetAuthenticationStateAsync();

        var user = authenticationState.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException(
                "The current user is not authenticated.");
        }

        var userId =
            user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException(
                "The current user does not have a valid user ID.");
        }

        return userId;
    }

    public async Task<List<Expense>> GetAllAsync(
        string? search = null,
        string? category = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        bool sortAscending = false)
    {
        var userId = await GetCurrentUserIdAsync();

        IQueryable<Expense> query =
            _dbContext.Expenses
                .Where(expense => expense.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(expense =>
                expense.Title.Contains(search)
                || (expense.Notes != null && expense.Notes.Contains(search))
            );
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(expense =>
                expense.Category == category);
        }

        if (startDate.HasValue)
        {
            query = query.Where(expense =>
                expense.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(expense =>
                expense.Date <= endDate.Value);
        }

        query = sortAscending
            ? query.OrderBy(expense => expense.Date)
            : query.OrderByDescending(expense => expense.Date);

        try
        {
            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to load expenses for user {UserId}.",
                userId);

            throw;
        }
    }

    public async Task<Expense> CreateAsync(Expense expense)
    {
        var userId = await GetCurrentUserIdAsync();

        expense.UserId = userId;
        expense.CreatedAt = DateTime.UtcNow;

        _dbContext.Expenses.Add(expense);

        await _dbContext.SaveChangesAsync();

        return expense;
    }

    public async Task UpdateAsync(Expense expense)
    {
        var userId = await GetCurrentUserIdAsync();

        var existingExpense =
            await _dbContext.Expenses
                .FirstOrDefaultAsync(existing =>
                    existing.Id == expense.Id
                    && existing.UserId == userId);

        if (existingExpense is null)
        {
            throw new InvalidOperationException(
                "The expense was not found.");
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
        var userId = await GetCurrentUserIdAsync();

        var expense =
            await _dbContext.Expenses
                .FirstOrDefaultAsync(existing =>
                    existing.Id == id
                    && existing.UserId == userId);

        if (expense is null)
        {
            return;
        }

        _dbContext.Expenses.Remove(expense);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<decimal> GetTotalSpendingAsync()
    {
        var userId = await GetCurrentUserIdAsync();

        var total =
            await _dbContext.Expenses
                .Where(expense => expense.UserId == userId)
                .SumAsync(expense => expense.Amount);

        return Math.Round(total, 3);
    }

    public async Task<decimal> GetCurrentMonthSpendingAsync()
    {
        var userId = await GetCurrentUserIdAsync();

        var now = DateTime.UtcNow;

        var startOfMonth =
            new DateTime(now.Year, now.Month, 1);

        var startOfNextMonth =
            startOfMonth.AddMonths(1);

        var total =
            await _dbContext.Expenses
                .Where(expense =>
                    expense.UserId == userId
                    && expense.Date >= startOfMonth
                    && expense.Date < startOfNextMonth)
                .SumAsync(expense => expense.Amount);

        return Math.Round(total, 3);
    }

    public async Task<int> GetExpenseCountAsync()
    {
        var userId = await GetCurrentUserIdAsync();

        return await _dbContext.Expenses
            .Where(expense => expense.UserId == userId)
            .CountAsync();
    }

    public async Task<Dictionary<string, decimal>>
        GetSpendingByCategoryAsync()
    {
        var userId = await GetCurrentUserIdAsync();

        return await _dbContext.Expenses
            .Where(expense => expense.UserId == userId)
            .GroupBy(expense => expense.Category)
            .Select(group => new
            {
                Category = group.Key,
                Total = Math.Round(
                    group.Sum(expense => expense.Amount),
                    3)
            })
            .ToDictionaryAsync(
                item => item.Category,
                item => item.Total);
    }

    public async Task<List<Expense>> GetRecentExpensesAsync(
        int count = 5)
    {
        var userId = await GetCurrentUserIdAsync();

        return await _dbContext.Expenses
            .Where(expense => expense.UserId == userId)
            .OrderByDescending(expense => expense.Date)
            .Take(count)
            .ToListAsync();
    }
}
