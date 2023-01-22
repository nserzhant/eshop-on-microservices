using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using R2S.EmployeeManagement.Api.Models;
using R2S.EmployeeManagement.Core.Exceptions;

namespace R2S.EmployeeManagement.Api.Filters
{
    public class UsersDomainExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is BaseEmployeeDomainException usersDomainException)
            {
                context.Result =
                    new BadRequestObjectResult(
                        new ApiErrorDTO(usersDomainException));

                context.ExceptionHandled = true;
            }
        }
    }
}
