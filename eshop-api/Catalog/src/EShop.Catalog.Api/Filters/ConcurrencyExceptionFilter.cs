using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace EShop.Catalog.Api.Filters;

public class ConcurrencyExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is DbUpdateConcurrencyException concurrencyException)
        {
            context.Result = new ConflictResult();

            context.ExceptionHandled = true;
        }
    }
}
