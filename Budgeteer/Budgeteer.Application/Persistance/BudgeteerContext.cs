using Budgeteer.Application.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Budgeteer.Application.Persistance;

public class BudgeteerContext : IdentityDbContext<User>
{
    public BudgeteerContext(DbContextOptions<BudgeteerContext> options) : base(options)
    {
        
    }
}