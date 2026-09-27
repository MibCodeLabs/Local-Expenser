using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalExpenseTracker.Models;

namespace PersonalExpenseTracker.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Expense> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Expense>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(expense => expense.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
