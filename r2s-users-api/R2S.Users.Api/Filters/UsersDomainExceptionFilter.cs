using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using R2S.Users.Api.Models;
using R2S.Users.Core.Exceptions;

namespace R2S.Users.Api.Filters
{
    public class UsersDomainExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is BaseUsersDomainException usersDomainException)
            {
                context.Result =
                    new BadRequestObjectResult(
                        new ApiErrorDTO(usersDomainException));

                context.ExceptionHandled = true;
            }
        }
    }
}
