namespace Budgeteer.Api.Models
{
    public class ResponseModelBase
    {
        public string ErrorMessage { get; set; }
        public List<string> Errors { get; set; }
    }
}