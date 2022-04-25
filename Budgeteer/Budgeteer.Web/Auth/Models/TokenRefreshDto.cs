namespace Budgeteer.Web.Auth.Models
{
    public class TokenRefreshDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}