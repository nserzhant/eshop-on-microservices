using EShop.EmployeeManagement.Infrastructure.Read.ReadModels;

namespace EShop.EmployeeManagement.Infrastructure.Read.Queries.Results;

public class ListEmployeeQueryResult
{
    public IList<EmployeeReadModel> Employees { get; set; }
    public int TotalCount { get; set; }
}
