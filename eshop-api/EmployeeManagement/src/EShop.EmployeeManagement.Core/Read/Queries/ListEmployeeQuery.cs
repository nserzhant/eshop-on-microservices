namespace EShop.EmployeeManagement.Core.Read.Queries;

public enum ListEmployeeOrderBy
{
    Email,
    UserName
}
public class ListEmployeeQuery
{
    public ListEmployeeOrderBy OrderBy { get; set; }
    public OrderByDirections OrderByDirection { get; set; }
    public int PageSize { get; set; }
    public int PageIndex { get; set; }
    public string? EmailFilter { get; set; }
}
