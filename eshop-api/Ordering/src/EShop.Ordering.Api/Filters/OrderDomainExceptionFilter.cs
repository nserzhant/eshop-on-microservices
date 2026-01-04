using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using EShop.Ordering.Core.Exceptions;
using EShop.Ordering.Api.Models;

namespace EShop.Ordering.Api.Filters;

public class OrderDomainExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is BaseOrderingDomainException catalogDomainException)
        {
            context.Result =
                new BadRequestObjectResult(
                    new OrderDomainErrorDTO(catalogDomainException));

            context.ExceptionHandled = true;
        }
    }
}