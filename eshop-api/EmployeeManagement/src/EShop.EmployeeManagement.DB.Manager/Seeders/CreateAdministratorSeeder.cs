using EShop.EmployeeManagement.Core.Enums;
using EShop.EmployeeManagement.Core.Read;
using EShop.EmployeeManagement.Core.Services;

namespace EShop.EmployeeManagement.DB.Manager.Seeders;

internal class CreateAdministratorSeeder
{
    private readonly IEmployeeService _employeeService = null;
    private readonly IEmployeeQueryService _employeeQueryService = null;

    public CreateAdministratorSeeder(IEmployeeService employeeService, IEmployeeQueryService employeeQueryService)
    {
        _employeeService = employeeService;
        _employeeQueryService = employeeQueryService;
    }

    public async Task Seed(string administratorEmail, string administratorPassword)
    {
        try
        {
            var result = await _employeeService.Register(administratorEmail, administratorPassword);

            if (!result.Succeeded)
            {
                Console.WriteLine("Failed to create administrator");
                foreach (var error in result.Errors)
                    Console.WriteLine($"{error.Code}    {error.Description}");

                return;
            }

            var administrator = await _employeeQueryService.GetByEmail(administratorEmail);
            await _employeeService.SetRoles(administrator.Id, Roles.Administrator);
        }
        catch (Exception e)
        {
            Console.Write(e);
        }
    }
}
