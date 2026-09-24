using Microsoft.EntityFrameworkCore;
using PersonalExpenseTracker.Models;

namespace PersonalExpenseTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Expense> Expenses => Set<Expense>();
}
