using EShop.EmployeeManagement.Infrastructure.Read.Queries;
using EShop.EmployeeManagement.Infrastructure.Read.Queries.Results;
using EShop.EmployeeManagement.Infrastructure.Read.ReadModels;

namespace EShop.EmployeeManagement.Infrastructure.Read;

public interface IEmployeeQueryService
{
    Task<EmployeeReadModel> GetById(Guid id);
    Task<ListEmployeeQueryResult> GetEmployees(ListEmployeeQuery listEmployeeQuery);
    Task<EmployeeReadModel> GetByEmail(string email);
}