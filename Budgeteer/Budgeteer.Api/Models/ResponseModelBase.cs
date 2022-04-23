using System.Collections.Generic;

namespace BudgetUs.Api.Models
{
    public class ResponseModelBase
    {
        public string GeneralErrorMessage { get; set; }
        public List<ErrorModel> Errors { get; set; }
    }
    
    public class ErrorModel
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}