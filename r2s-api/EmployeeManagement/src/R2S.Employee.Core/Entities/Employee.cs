using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R2S.EmployeeManagement.Core.Entities
{
    public class Employee : IdentityUser<Guid>
    {
    }
}