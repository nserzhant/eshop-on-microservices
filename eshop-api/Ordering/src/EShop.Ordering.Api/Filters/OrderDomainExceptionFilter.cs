using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using EShop.Ordering.Core.Exceptions;
using EShop.Ordering.Api.Models;

namespace EShop.Ordering.Api.Filters;

public class OrderDomainExceptionFilter : IActionFilter, IOrderedFilter
{
    private ILogger<OrderDomainExceptionFilter> _logger;
    public int Order => int.MaxValue - 10;

    public OrderDomainExceptionFilter(ILogger<OrderDomainExceptionFilter> logger)
    { 
        _logger = logger; 
    }

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is BaseOrderingDomainException catalogDomainException)
        {
            context.Result =
                new BadRequestObjectResult(
                    new OrderDomainErrorDTO(catalogDomainException));

            context.ExceptionHandled = true;

            _logger.LogError($"Domain exception wat thrown: {context.Exception.GetType().Name}");
        }
    }
}