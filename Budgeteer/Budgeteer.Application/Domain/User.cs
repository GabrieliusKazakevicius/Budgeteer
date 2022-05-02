namespace Budgeteer.Application.Domain;

public class User
{
    public long Id { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public string RefreshToken { get; set; }
        
    public IList<Account> Accounts { get; set; }
}