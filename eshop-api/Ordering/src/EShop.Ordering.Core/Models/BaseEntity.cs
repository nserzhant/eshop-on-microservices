namespace EShop.Ordering.Core.Models;
public abstract class BaseEntity
{
    public byte[] Ts { get; protected set; }
    public virtual int Id { get; protected set; }
}