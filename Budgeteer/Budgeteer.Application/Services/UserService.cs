using Budgeteer.Application.Domain;
using Budgeteer.Application.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Budgeteer.Application.Services;

public class UserService
{
    public BudgeteerContext _dbContext { get; set; }
    
    public UserService(BudgeteerContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<ServiceResult<User, AuthServiceError>> FindByNameAsync(string username)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username));
        if (user is null)
            return AuthServiceError.NotFound;
        
        return user;
    }

    public Task<ServiceResult<bool, AuthServiceError>> CheckPasswordAsync(User user, string password)
    {
        var result = user.PasswordHash == CalculatePasswordHash(password) 
            ? (ServiceResult<bool, AuthServiceError>) true 
            : AuthServiceError.InvalidCredentials;
        
        return Task.FromResult(result);
    }

    private string CalculatePasswordHash(string password)
    {
        
    }
}

public enum AuthServiceError    
{
    NotFound,
    InvalidCredentials
}