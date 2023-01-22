using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R2S.EmployeeManagement.Core.Read.ReadModels
{
    public class EmployeeReadModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<RoleReadModel> Roles { get; set; }
    }
}
