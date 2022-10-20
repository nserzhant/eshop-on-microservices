using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R2S.Users.Core.Entities
{
    public class User : IdentityUser<Guid>
    {
    }
}