using Microsoft.EntityFrameworkCore;
using EShop.EmployeeManagement.Infrastructure.Read.Queries;
using EShop.EmployeeManagement.Infrastructure.Read.Queries.Results;
using EShop.EmployeeManagement.Infrastructure.Read.ReadModels;
using System.Linq.Dynamic.Core;

namespace EShop.EmployeeManagement.Infrastructure.Read;

public class EmployeeQueryService : IEmployeeQueryService
{
    private readonly EmployeeReadDbContext _employeeReadDbContext;

    public EmployeeQueryService(EmployeeReadDbContext employeeReadDbContext)
    {
        _employeeReadDbContext = employeeReadDbContext;
    }

    public async Task<EmployeeReadModel> GetByEmail(string email)
    {
        var employee = await _employeeReadDbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == email);

        return employee;
    }

    public async Task<EmployeeReadModel> GetById(Guid employeeId)
    {
        var employee = await _employeeReadDbContext.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Id == employeeId);

        return employee;
    }

    public async Task<ListEmployeeQueryResult> GetEmployees(ListEmployeeQuery listEmployeeQuery)
    {
        var count = _employeeReadDbContext.Users
            .Where(u => 
                listEmployeeQuery.EmailFilter == null 
                || u.Email.Contains(listEmployeeQuery.EmailFilter)).Count();
        var orderByExpression = $"{listEmployeeQuery.OrderBy} {listEmployeeQuery.OrderByDirection}";

        var employees = await _employeeReadDbContext.Users.Include(u => u.Roles)
            .Where(u => 
                listEmployeeQuery.EmailFilter == null 
                || u.Email.Contains(listEmployeeQuery.EmailFilter))
            .OrderBy(orderByExpression)
            .Skip(listEmployeeQuery.PageIndex * listEmployeeQuery.PageSize)
            .Take(listEmployeeQuery.PageSize).ToListAsync();

        var result = new ListEmployeeQueryResult()
        {
            TotalCount = count,
            Employees = employees
        };

        return result;
    }
}
