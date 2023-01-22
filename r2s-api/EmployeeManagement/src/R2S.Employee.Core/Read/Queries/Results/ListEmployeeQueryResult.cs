using R2S.EmployeeManagement.Core.Read.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R2S.EmployeeManagement.Core.Read.Queries.Results
{
    public class ListEmployeeQueryResult
    {
        public IList<EmployeeReadModel> Users { get; set; }
        public int TotalCount { get; set; }
    }
}
