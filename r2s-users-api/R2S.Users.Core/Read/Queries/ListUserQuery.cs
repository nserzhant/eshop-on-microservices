using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R2S.Users.Core.Read.Queries
{
    public enum ListUserOrderBy
    {
        Email,
        UserName
    }
    public class ListUserQuery
    {
        public ListUserOrderBy OrderBy { get; set; }
        public OrderByDirections OrderByDirection { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string? EmailFilter { get; set; }
    }
}
