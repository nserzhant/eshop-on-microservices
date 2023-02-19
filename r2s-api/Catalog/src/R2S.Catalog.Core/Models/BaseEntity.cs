using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R2S.Catalog.Core.Models;

public abstract class BaseEntity
{
    public byte[] Ts { get; protected set; }
    public virtual Guid Id { get; protected set; }
}
