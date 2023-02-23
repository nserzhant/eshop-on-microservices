using R2S.Catalog.Core.Exceptions;

namespace R2S.Catalog.Api.Models;

public class CatalogDomainErrorDTO
{    public string ErrorType { get; private set; }

    public CatalogDomainErrorDTO(BaseCatalogDomainException applicationException)
    {
        ErrorType = camelize(applicationException.GetType().Name);
    }

    private string camelize(string name)
    {
        var camelized = Char.ToLowerInvariant(name[0]) + name.Substring(1);
        return camelized;
    }
}
