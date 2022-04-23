using System;

namespace BudgetUs.Api.Models.Auth
{
    public class LoginResponseViewModel : ResponseModelBase
    {
        public string Token { get; set; }
        public DateTime TokenValidUntil { get; set; }
        
        public string RefreshToken { get; set; }
    }
}