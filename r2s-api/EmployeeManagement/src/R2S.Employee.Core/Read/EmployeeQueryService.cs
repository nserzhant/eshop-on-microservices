using Microsoft.EntityFrameworkCore;
using R2S.EmployeeManagement.Core.Read.Queries;
using R2S.EmployeeManagement.Core.Read.Queries.Results;
using R2S.EmployeeManagement.Core.Read.ReadModels;
using System.Linq.Dynamic.Core;

namespace R2S.EmployeeManagement.Core.Read
{
    public class EmployeeQueryService : IEmployeeQueryService
    {
        private readonly EmployeeReadDbContext _usersReadDbContext;

        public EmployeeQueryService(EmployeeReadDbContext usersReadDbContext)
        {
            _usersReadDbContext = usersReadDbContext;
        }

        public async Task<EmployeeReadModel> GetByEmail(string userEmail)
        {
            var user = await _usersReadDbContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserName == userEmail);

            return user;
        }

        public async Task<EmployeeReadModel> GetById(Guid userId)
        {
            var user = await _usersReadDbContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user;
        }

        public async Task<ListEmployeeQueryResult> GetUsers(ListEmployeeQuery listUserQuery)
        {
            var count = _usersReadDbContext.Users
                .Where(u => 
                    listUserQuery.EmailFilter == null 
                    || u.Email.Contains(listUserQuery.EmailFilter)).Count();
            var orderByExpression = $"{listUserQuery.OrderBy} {listUserQuery.OrderByDirection}";

            var users = await _usersReadDbContext.Users.Include(u => u.Roles)
                .Where(u => 
                    listUserQuery.EmailFilter == null 
                    || u.Email.Contains(listUserQuery.EmailFilter))
                .OrderBy(orderByExpression)
                .Skip(listUserQuery.PageIndex * listUserQuery.PageSize)
                .Take(listUserQuery.PageSize).ToListAsync();

            var result = new ListEmployeeQueryResult()
            {
                TotalCount = count,
                Users = users
            };

            return result;
        }
    }
}
