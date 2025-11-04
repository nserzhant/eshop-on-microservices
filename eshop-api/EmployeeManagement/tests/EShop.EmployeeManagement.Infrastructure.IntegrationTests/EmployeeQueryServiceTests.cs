using EShop.EmployeeManagement.Infrastructure.Enums;
using EShop.EmployeeManagement.Infrastructure.Read;
using EShop.EmployeeManagement.Infrastructure.Read.Queries;
using EShop.EmployeeManagement.Infrastructure.Read.ReadModels;
using Microsoft.Extensions.DependencyInjection;

namespace EShop.EmployeeManagement.Infrastructure.IntegrationTests;


[TestFixture]
[Category("User Query Service")]
public class EmployeeQueryServiceTests : BaseEmployeeIntegrationTests
{
    private IEmployeeQueryService _employeeQueryService;

    [SetUp]
    public override async Task Setup()
    {
        await base.Setup();

        _employeeQueryService = serviceProvier.GetRequiredService<IEmployeeQueryService>();
    }

    [Test]
    [Category("GetByEmail")]
    public async Task When_Get_Employee_By_Email_Then_Employee_Should_Be_Returned()
    {
        var email = "newemail@test.com";
        await createEmployee(email);

        EmployeeReadModel user = await _employeeQueryService.GetByEmail(email);

        Assert.That(user, Is.Not.Null);
        Assert.That(user.Email, Is.EqualTo(email));
    }

    [Test]
    [Category("GetById")]
    public async Task When_Get_Employee_By_Id_Then_Employee_Should_Be_Returned()
    {
        var email = "newemail@test.com";
        var employeeId = await createEmployee(email);

        var result = await _employeeQueryService.GetById(employeeId);

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    [Category("GetEmployees")]
    [TestCase(ListEmployeeOrderBy.Email, OrderByDirections.ASC, "newemail0@test.com", 2)]
    [TestCase(ListEmployeeOrderBy.Email, OrderByDirections.DESC, "newemail5@test.com", 1)]
    public async Task When_List_Employees_Then_They_Can_Be_Ordered_By_Email(ListEmployeeOrderBy orderBy, OrderByDirections directions, string resultingFirstEmail, int rolesCount)
    {
        await createEmployee("newemail1test.com", Roles.Administrator, Roles.SalesManager);
        await createEmployee("newemail3@test.com", Roles.SalesManager);
        await createEmployee("newemail0@test.com", Roles.Administrator, Roles.SalesManager);
        await createEmployee("newemail5@test.com", Roles.SalesManager);
        await createEmployee("newemail2@test.com", Roles.Administrator, Roles.SalesManager);
        var query = new ListEmployeeQuery();
        query.OrderBy = orderBy;
        query.OrderByDirection = directions;
        query.PageSize = 2;
        query.PageIndex = 0;

        var resultAsc = await _employeeQueryService.GetEmployees(query);

        Assert.That(resultAsc, Is.Not.Null);
        Assert.That(resultAsc.TotalCount, Is.EqualTo(5));
        Assert.That(resultAsc.Employees.Count, Is.EqualTo(2));
        Assert.That(resultAsc.Employees[0].Email, Is.EqualTo(resultingFirstEmail));
        Assert.That(resultAsc.Employees[0].Roles, Is.Not.Null);
        Assert.That(resultAsc.Employees[0].Roles.Count, Is.EqualTo(rolesCount));
    }


    [Test]
    [Category("GetEmployees")]
    public async Task When_List_Employees_Then_They_Can_Be_Filtered_By_Email()
    {
        await createEmployee("newemail0@test.com", Roles.Administrator);
        await createEmployee("anotherEmail@test.com", Roles.SalesManager);
        await createEmployee("newemail4@test.com", Roles.Administrator);
        await createEmployee("filteredEmail1@test.com", Roles.SalesManager);
        await createEmployee("newemail1@test.com", Roles.Administrator);
        await createEmployee("newemail2@test.com", Roles.SalesManager);
        await createEmployee("additionalFilteredEmail@test.com", Roles.Administrator);
        var query = new ListEmployeeQuery();
        query.OrderBy = ListEmployeeOrderBy.Email;
        query.OrderByDirection = OrderByDirections.ASC;
        query.PageSize = 5;
        query.PageIndex = 0;
        query.EmailFilter = "filter";

        var resultAsc = await _employeeQueryService.GetEmployees(query);

        Assert.That(resultAsc, Is.Not.Null);
        Assert.That(resultAsc.TotalCount, Is.EqualTo(2));
        Assert.That(resultAsc.Employees.Count, Is.EqualTo(2));
        Assert.That(resultAsc.Employees[0].Email, Is.EqualTo("additionalFilteredEmail@test.com"));
        Assert.That(resultAsc.Employees[1].Email, Is.EqualTo("filteredEmail1@test.com"));
    }
}