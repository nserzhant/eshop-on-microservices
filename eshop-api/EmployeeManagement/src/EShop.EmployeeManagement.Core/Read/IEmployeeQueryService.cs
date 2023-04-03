using EShop.EmployeeManagement.Core.Read.Queries;
using EShop.EmployeeManagement.Core.Read.Queries.Results;
using EShop.EmployeeManagement.Core.Read.ReadModels;

namespace EShop.EmployeeManagement.Core.Read;

public interface IEmployeeQueryService
{
    Task<EmployeeReadModel> GetById(Guid id);
    Task<ListEmployeeQueryResult> GetEmployees(ListEmployeeQuery listEmployeeQuery);
    Task<EmployeeReadModel> GetByEmail(string email);
}