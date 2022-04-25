namespace Budgeteer.Web.Auth.Models
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }
    }
}