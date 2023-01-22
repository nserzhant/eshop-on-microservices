using R2S.EmployeeManagement.Core.Read.Queries;
using R2S.EmployeeManagement.Core.Read.Queries.Results;
using R2S.EmployeeManagement.Core.Read.ReadModels;

namespace R2S.EmployeeManagement.Core.Read
{
    public interface IEmployeeQueryService
    {
        Task<EmployeeReadModel> GetById(Guid id);
        Task<ListEmployeeQueryResult> GetUsers(ListEmployeeQuery listUserQuery);
        Task<EmployeeReadModel> GetByEmail(string userEmail);
    }
}