using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using EShop.Catalog.Core.Exceptions;
using EShop.Catalog.Api.Models;

namespace EShop.Catalog.Api.Filters;

public class CatalogDomainExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is BaseCatalogDomainException catalogDomainException)
        {
            context.Result =
                new BadRequestObjectResult(
                    new CatalogDomainErrorDTO(catalogDomainException));

            context.ExceptionHandled = true;
        }
    }
}
