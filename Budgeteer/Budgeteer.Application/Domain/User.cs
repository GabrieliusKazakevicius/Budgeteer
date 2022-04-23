using Microsoft.AspNetCore.Identity;

namespace Budgeteer.Application.Domain;

public class User : IdentityUser
{
    public string RefreshToken { get; set; }
        
    public IList<Account> Accounts { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}