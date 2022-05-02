using Budgeteer.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Budgeteer.Application.Persistance;

public class BudgeteerContext : DbContext
{
    public BudgeteerContext(DbContextOptions<BudgeteerContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
}