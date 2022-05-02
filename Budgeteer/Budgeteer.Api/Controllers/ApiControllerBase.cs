using Budgeteer.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Budgeteer.Api.Controllers;

[ApiController]
public class ApiControllerBase : Controller
{
    public ActionResult Unauthorized(Enum error, string errorMessage)
    {
        var response = new ResponseModelBase
        {
            Errors = new List<string> { error.ToString() },
            ErrorMessage = errorMessage
        };

        return Unauthorized(response);
    }
}