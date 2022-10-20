using R2S.Users.Core.Read.ReadModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R2S.Users.Core.Read.Queries.Results
{
    public class ListUserQueryResult
    {
        public IList<UserReadModel> Users { get; set; }
        public int TotalCount { get; set; }
    }
}
