namespace Budgeteer.Application.Domain;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Currency Currency { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public User Owner { get; set; }
}