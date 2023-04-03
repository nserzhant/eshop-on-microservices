using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using EShop.EmployeeManagement.Api.Models;
using EShop.EmployeeManagement.Core.Exceptions;

namespace EShop.EmployeeManagement.Api.Filters;

public class EmployeeDomainExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is BaseEmployeeDomainException employeeDomainException)
        {
            context.Result =
                new BadRequestObjectResult(
                    new ApiErrorDTO(employeeDomainException));

            context.ExceptionHandled = true;
        }
    }
}
