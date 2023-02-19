using R2S.Catalog.Core.Exceptions;

namespace R2S.Catalog.Core.Models;

public class CatalogType : BaseEntity
{
    public string Type { get; private set; }
    public CatalogType(string type) => UpdateType(type);

    public void UpdateType(string type)
    {
        if (string.IsNullOrEmpty(type))
            throw new CatalogTypeIsNullOrEmptyException();

        Type = type;
    }

    public void UpdateTs(byte[] ts)
    {
        if (ts == null)
            throw new CatalogTypeTsIsNullException();

        Ts = ts;
    }
}
