namespace R2S.Catalog.Core.Models;

public abstract class BaseEntity
{
    public byte[] Ts { get; protected set; }
    public virtual Guid Id { get; protected set; }

    public BaseEntity() 
    {
        Id = Guid.NewGuid();
    }
}
