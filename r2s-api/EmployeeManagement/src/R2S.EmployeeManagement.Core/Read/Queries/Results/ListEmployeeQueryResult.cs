using R2S.EmployeeManagement.Core.Read.ReadModels;

namespace R2S.EmployeeManagement.Core.Read.Queries.Results;

public class ListEmployeeQueryResult
{
    public IList<EmployeeReadModel> Employees { get; set; }
    public int TotalCount { get; set; }
}
