namespace Budgeteer.Application.Domain;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; }
        
    public User Owner { get; set; }
}