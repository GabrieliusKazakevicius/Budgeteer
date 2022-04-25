namespace Budgeteer.Web.Auth.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public DateTime TokenValidUntil { get; set; }
        public string RefreshToken { get; set; }
    }
}