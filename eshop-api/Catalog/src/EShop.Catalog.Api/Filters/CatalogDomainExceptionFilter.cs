using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using EShop.Catalog.Core.Exceptions;
using EShop.Catalog.Api.Models;

namespace EShop.Catalog.Api.Filters;

public class CatalogDomainExceptionFilter : IExceptionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnException(ExceptionContext context)
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
