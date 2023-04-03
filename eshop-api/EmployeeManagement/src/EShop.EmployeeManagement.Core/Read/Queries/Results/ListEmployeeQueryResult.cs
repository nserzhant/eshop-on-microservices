using EShop.EmployeeManagement.Core.Read.ReadModels;

namespace EShop.EmployeeManagement.Core.Read.Queries.Results;

public class ListEmployeeQueryResult
{
    public IList<EmployeeReadModel> Employees { get; set; }
    public int TotalCount { get; set; }
}
