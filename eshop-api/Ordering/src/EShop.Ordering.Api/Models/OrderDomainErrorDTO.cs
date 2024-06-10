using EShop.Ordering.Core.Exceptions;

namespace EShop.Ordering.Api.Models;

public class OrderDomainErrorDTO
{
    public string ErrorType { get; private set; }

    public OrderDomainErrorDTO(BaseOrderingDomainException applicationException)
    {
        ErrorType = camelize(applicationException.GetType().Name);
    }

    private string camelize(string name)
    {
        var camelized = Char.ToLowerInvariant(name[0]) + name.Substring(1);
        return camelized;
    }
}
