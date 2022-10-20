using R2S.Users.Core.Read.Queries;
using R2S.Users.Core.Read.Queries.Results;
using R2S.Users.Core.Read.ReadModels;

namespace R2S.Users.Core.Read
{
    public interface IUserQueryService
    {
        Task<UserReadModel> GetById(Guid id);
        Task<ListUserQueryResult> GetUsers(ListUserQuery listUserQuery);
        Task<UserReadModel> GetByEmail(string userEmail);
    }
}