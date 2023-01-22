using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace R2S.EmployeeManagement.Core.Read.ReadModels
{
    public class RoleReadModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public List<EmployeeReadModel> Users { get; set; }
    }
}
