using Microsoft.EntityFrameworkCore;
using R2S.Users.Core.Read.Queries;
using R2S.Users.Core.Read.Queries.Results;
using R2S.Users.Core.Read.ReadModels;
using System.Linq.Dynamic.Core;

namespace R2S.Users.Core.Read
{
    public class UserQueryService : IUserQueryService
    {
        private readonly UsersReadDbContext _usersReadDbContext;

        public UserQueryService(UsersReadDbContext usersReadDbContext)
        {
            _usersReadDbContext = usersReadDbContext;
        }

        public async Task<UserReadModel> GetByEmail(string userEmail)
        {
            var user = await _usersReadDbContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserName == userEmail);

            return user;
        }

        public async Task<UserReadModel> GetById(Guid userId)
        {
            var user = await _usersReadDbContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user;
        }

        public async Task<ListUserQueryResult> GetUsers(ListUserQuery listUserQuery)
        {
            var count = _usersReadDbContext.Users.Count();
            var orderByExpression = $"{listUserQuery.OrderBy} {listUserQuery.OrderByDirection}";

            var users = await _usersReadDbContext.Users.Include(u => u.Roles)
                .OrderBy(orderByExpression)
                .Take(listUserQuery.PageSize)
                .Skip(listUserQuery.PageIndex * listUserQuery.PageSize).ToListAsync();

            var result = new ListUserQueryResult()
            {
                TotalCount = count,
                Users = users
            };

            return result;
        }
    }
}
