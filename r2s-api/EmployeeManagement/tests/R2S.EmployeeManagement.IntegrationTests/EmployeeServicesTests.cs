using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using R2S.EmployeeManagement.Core.Enums;
using R2S.EmployeeManagement.Core.Exceptions;
using R2S.EmployeeManagement.Core.Read;
using R2S.EmployeeManagement.Core.Read.Queries;
using R2S.EmployeeManagement.Core.Read.ReadModels;
using R2S.EmployeeManagement.Core.Services;

namespace R2S.EmployeeManagement.Core.IntegrationTests;


[TestFixture]
public class EmployeeServicesTests : BaseEmployeeIntegrationTests
{
    protected IEmployeeService employeeService;
    protected EmployeeReadDbContext readDbContext;
    protected IEmployeeQueryService employeeQueryService;

    [SetUp]
    public override async Task Setup()
    {
        await base.Setup();

        employeeService = serviceProvier.GetRequiredService<IEmployeeService>();
        readDbContext = serviceProvier.GetRequiredService<EmployeeReadDbContext>();
        employeeQueryService = serviceProvier.GetRequiredService<IEmployeeQueryService>();
    }

    [Test]
    [Category("Employee Service")]
    [Category("Register")]
    public void When_Register_Employee_With_Valid_Credentials_Then_Employee_Should_Be_Created()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";

        Task act() => employeeService.Register(email, password);

        Assert.That(act, Throws.Nothing);
    }

    [Test]
    [Category("Employee Service")]
    [Category("Register")]
    public async Task Whent_Trying_To_Register_Employee_With_The_Same_Email_Correct_Error_Should_Be_Returned()
    {
        var email = "samople@test.com";
        await registerEmployee(email);

        var result = await  employeeService.Register(email, "abcdef123Av#");

        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.Errors.First().Code, Is.EqualTo(nameof(IdentityErrorDescriber.DuplicateUserName)));
    }

    [Test]
    [Category("Employee Service")]
    [Category("Login")]
    public async Task When_Employee_Is_Registered_Then_Login_Will_Be_Successfull()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);

        var claims = await employeeService.Login(email, password);

        Assert.That(claims, Is.Not.Null);
    }


    [Test]
    [Category("Employee Service")]
    [Category("Login")]
    public async Task When_Trying_To_Login_With_Invalid_Email_Then_Correct_Exception_Should_Be_Thrown()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);

        Task act() => employeeService.Login("newemail1@test.com", password);

        Assert.That(act, Throws.TypeOf<InvalidEmailOrPasswordException>());
    }


    [Test]
    [Category("Employee Service")]
    [Category("Login")]
    public async Task When_Trying_To_Login_With_Invalid_Password_Then_Correct_Exception_Should_Be_Thrown()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);

        Task act() => employeeService.Login(email, "invalidpassword");

        Assert.That(act, Throws.TypeOf<InvalidEmailOrPasswordException>());
    }

    [Test]
    [Category("User Query Service")]
    [Category("GetByEmail")]
    public async Task When_Employee_Exists_Then_It_Could_Be_Requested_By_Email()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);

        EmployeeReadModel user = await employeeQueryService.GetByEmail(email);

        Assert.That(user, Is.Not.Null);
        Assert.That(user.Email, Is.EqualTo(email));
    }

    [Test]
    [Category("User Query Service")]
    [Category("GetById")]
    public async Task When_Employee_Exists_Then_It_Could_Be_Requested_By_Id()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);
        var employee = await employeeQueryService.GetByEmail(email);

        var result = await employeeQueryService.GetById(employee.Id);

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    [Category("Employee Service")]
    [Category("Save Roles")]
    public async Task When_Saving_A_Single_Role_For_The_Employee_Then_Employee_Will_Contains_Only_This_Role()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);
        var userReadModel = await employeeQueryService.GetByEmail(email);

        await employeeService.SetRoles(userReadModel.Id, Roles.Administrator);
        var employee = await employeeQueryService.GetByEmail(email);

        Assert.That(employee, Is.Not.Null);
        Assert.That(employee.Roles, Is.Not.Null);
        Assert.That(employee.Roles.Count, Is.EqualTo(1));
        Assert.That(employee.Roles[0].Name, Is.EqualTo(Roles.Administrator.ToString()));
        Assert.That(employee.Email, Is.EqualTo(email));
    }

    [Test]
    [Category("Employee Service")]
    [Category("Save Roles")]
    public async Task When_Saving_Two_Roles_For_The_Employee_Then_Employee_Will_Contains_Both_Roles()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);
        var userReadModel = await employeeQueryService.GetByEmail(email);

        await employeeService.SetRoles(userReadModel.Id, Roles.Administrator, Roles.SalesManager);
        var user = await employeeQueryService.GetByEmail(email);

        Assert.That(user, Is.Not.Null);
        Assert.That(user.Roles, Is.Not.Null);
        Assert.That(user.Roles.Count, Is.EqualTo(2));
        Assert.That(user.Roles.Select(r => r.Name), Does.Contain(Roles.Administrator.ToString()));
        Assert.That(user.Roles.Select(r => r.Name), Does.Contain(Roles.SalesManager.ToString()));
    }

    [Test]
    [Category("Employee Service")]
    [Category("Save Roles")]
    public async Task When_Adding_Additional_Role_For_The_Employee_Then_Employee_Will_Contains_Both_Roles()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);
        var employeeReadModel = await employeeQueryService.GetByEmail(email);
        await employeeService.SetRoles(employeeReadModel.Id, Roles.SalesManager);

        var result = await employeeService.SetRoles(employeeReadModel.Id, Roles.Administrator, Roles.SalesManager);
        var employee = await employeeQueryService.GetByEmail(email);

        Assert.That(employee, Is.Not.Null);
        Assert.That(employee.Roles, Is.Not.Null);
        Assert.That(employee.Roles.Count, Is.EqualTo(2));
        Assert.That(employee.Roles.Select(r => r.Name), Does.Contain(Roles.Administrator.ToString()));
        Assert.That(employee.Roles.Select(r => r.Name), Does.Contain(Roles.SalesManager.ToString()));
    }

    [Test]
    [Category("Employee Service")]
    [Category("Save Roles")]
    public async Task When_Saving_A_Single_Role_For_The_Employee_With_Two_Roles_Then_Employee_Will_Contains_Only_This_Role()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);
        var employeeReadModel = await employeeQueryService.GetByEmail(email);
        await employeeService.SetRoles(employeeReadModel.Id, Roles.Administrator, Roles.SalesManager);

        await employeeService.SetRoles(employeeReadModel.Id, Roles.SalesManager);
        var employee = await employeeQueryService.GetByEmail(email);

        Assert.That(employee, Is.Not.Null);
        Assert.That(employee.Roles, Is.Not.Null);
        Assert.That(employee.Roles.Count, Is.EqualTo(1));
        Assert.That(employee.Roles.Select(r => r.Name), Does.Contain(Roles.SalesManager.ToString()));
    }

    [Test]
    [Category("Employee Service")]
    [Category("Save Roles")]
    public async Task When_Trying_To_Trying_To_Save_Role_For_Unexisted_Employee_Then_Correct_Exception_Should_Be_Thrown()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);

        Task act() => employeeService.SetRoles(Guid.Empty, Roles.Administrator, Roles.SalesManager);

        Assert.That(act, Throws.TypeOf<EmployeeNotFoundException>());
    }

    [Test]
    [Category("User Query Service")]
    [Category("GetEmployees")]
    public async Task When_Users_Exists_Then_They_Could_Be_Requested_By_List_Query()
    {
        await registerEmployee("newemail1test.com", Roles.Administrator, Roles.SalesManager);
        await registerEmployee("newemail3@test.com", Roles.SalesManager);
        await registerEmployee("newemail0@test.com", Roles.Administrator, Roles.SalesManager);
        await registerEmployee("newemail5@test.com", Roles.SalesManager);
        await registerEmployee("newemail2@test.com", Roles.Administrator, Roles.SalesManager);
        var query = new ListEmployeeQuery();
        query.OrderBy = ListEmployeeOrderBy.Email;
        query.OrderByDirection = OrderByDirections.ASC;
        query.PageSize = 2;
        query.PageIndex = 0;

        var resultAsc = await employeeQueryService.GetEmployees(query);
        query.OrderByDirection = OrderByDirections.DESC;
        var resultDesc = await employeeQueryService.GetEmployees(query);

        Assert.That(resultAsc, Is.Not.Null);
        Assert.That(resultAsc.TotalCount, Is.EqualTo(5));
        Assert.That(resultAsc.Employees.Count, Is.EqualTo(2));
        Assert.That(resultAsc.Employees[0].Email, Is.EqualTo("newemail0@test.com"));
        Assert.That(resultAsc.Employees[0].Roles, Is.Not.Null);
        Assert.That(resultAsc.Employees[0].Roles.Count, Is.EqualTo(2));

        Assert.That(resultDesc, Is.Not.Null);
        Assert.That(resultDesc.TotalCount, Is.EqualTo(5));
        Assert.That(resultDesc.Employees.Count, Is.EqualTo(2));
        Assert.That(resultDesc.Employees[0].Email, Is.EqualTo("newemail5@test.com"));
        Assert.That(resultDesc.Employees[0].Roles, Is.Not.Null);
        Assert.That(resultDesc.Employees[0].Roles.Count, Is.EqualTo(1));
    }


    [Test]
    [Category("User Query Service")]
    [Category("GetEmployees")]
    public async Task When_Users_Exists_Then_They_Could_Be_Filtered_By_List_Query()
    {
        await registerEmployee("newemail0@test.com", Roles.Administrator);
        await registerEmployee("anotherEmail@test.com", Roles.SalesManager);
        await registerEmployee("newemail0@test.com", Roles.Administrator);
        await registerEmployee("filteredEmail1@test.com", Roles.SalesManager);
        await registerEmployee("newemail1@test.com", Roles.Administrator);
        await registerEmployee("newemail2@test.com", Roles.SalesManager);
        await registerEmployee("additionalFilteredEmail@test.com", Roles.Administrator);
        var query = new ListEmployeeQuery();
        query.OrderBy = ListEmployeeOrderBy.Email;
        query.OrderByDirection = OrderByDirections.ASC;
        query.PageSize = 5;
        query.PageIndex = 0;
        query.EmailFilter = "filter";

        var resultAsc = await employeeQueryService.GetEmployees(query);

        Assert.That(resultAsc, Is.Not.Null);
        Assert.That(resultAsc.TotalCount, Is.EqualTo(2));
        Assert.That(resultAsc.Employees.Count, Is.EqualTo(2));
        Assert.That(resultAsc.Employees[0].Email, Is.EqualTo("additionalFilteredEmail@test.com"));
        Assert.That(resultAsc.Employees[1].Email, Is.EqualTo("filteredEmail1@test.com"));
    }

    [Test]
    [Category("Employee Service")]
    [Category("SetPassword")]
    public async Task When_Password_Was_Set_Then_Employee_Could_Login_Using_It()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);
        var employeeReadModel = await employeeQueryService.GetByEmail(email);
        var newPassword = "abcdef123Av#$3e!";

        await employeeService.SetPassword(employeeReadModel.Id, newPassword);
      
        async Task assert() => await employeeService.Login(email, newPassword);
        Assert.That(assert, Throws.Nothing);
    }

    [Test]
    [Category("Employee Service")]
    [Category("SetPassword")]
    public void When_Trying_To_Set_Password_For_Non_Existing_Employee_Then_Exception_Should_Be_Thrown()
    {
        var nonExistingId = Guid.NewGuid();
        var newPassword = "abcdef123Av#$3e!";

        Task act() => employeeService.SetPassword(nonExistingId, newPassword);

        Assert.That(act, Throws.TypeOf<EmployeeNotFoundException>());
    }

    [Test]
    [Category("Employee Service")]
    [Category("ChangePassword")]
    public async Task When_Password_Was_Changed_Then_Employee_Could_Login_Using_It()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);
        var employeeReadModel = await employeeQueryService.GetByEmail(email);
        var newPassword = "abcdef123Av#$3e!";

        await employeeService.ChangePassword(employeeReadModel.Id, password, newPassword);

        async Task assert() => await employeeService.Login(email, newPassword);
        Assert.That(assert, Throws.Nothing);
    }

    [Test]
    [Category("Employee Service")]
    [Category("ChangePassword")]
    public void When_Trying_To_Change_Password_For_Non_Existing_Employee_Then_Exception_Should_Be_Thrown()
    {
        var nonExistingId = Guid.NewGuid();
        var poassword = "abcdef123Av#";
        var newPassword = "abcdef123Av#$3e!";

        Task act() => employeeService.ChangePassword(nonExistingId, poassword, newPassword);

        Assert.That(act, Throws.TypeOf<EmployeeNotFoundException>());
    }

    [Test]
    [Category("Employee Service")]
    [Category("ChangePassword")]
    public async Task When_Trying_To_Change_Password_With_Wrong_Old_Password_Then_Exception_Should_Be_Thrown()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);
        var employeeReadModel = await employeeQueryService.GetByEmail(email);
        var newPassword = "abcdef123Av#$3e!";

        Task act() => employeeService.ChangePassword(employeeReadModel.Id, $"{password}_wrong", newPassword);

        Assert.That(act, Throws.TypeOf<InvalidPasswordException>());
    }

    [Test]
    [Category("Employee Service")]
    [Category("ChangeEmail")]
    public async Task When_Email_Was_Changed_Then_Employee_Could_Login_Using_It()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);
        var employeeReadModel = await employeeQueryService.GetByEmail(email);
        var newEmail = "newemail_changed@test.com";

        await employeeService.ChangeEmail(employeeReadModel.Id, newEmail, password);

        async Task assert() => await employeeService.Login(newEmail, password);
        Assert.That(assert, Throws.Nothing);
    }

    [Test]
    [Category("Employee Service")]
    [Category("ChangeEmail")]
    public void When_Trying_To_Change_Email_For_Non_Existing_Employee_Then_Exception_Should_Be_Thrown()
    {
        var nonExistingId = Guid.NewGuid();
        var newEmail = "newemail_changed@test.com";
        var password = "abcdef123Av#";

        Task act() => employeeService.ChangeEmail(nonExistingId, newEmail, password);

        Assert.That(act, Throws.TypeOf<EmployeeNotFoundException>());
    }

    [Test]
    [Category("Employee Service")]
    [Category("ChangeEmail")]
    public async Task When_Trying_To_Change_Email_With_Wrong_Old_Password_Then_Exception_Should_Be_Thrown()
    {
        var email = "newemail@test.com";
        var password = "abcdef123Av#";
        await employeeService.Register(email, password);
        var employeeReadModel = await employeeQueryService.GetByEmail(email);
        var newEmail = "newemail_changed@test.com";

        Task act() => employeeService.ChangeEmail(employeeReadModel.Id, newEmail, $"{password}_wrong");

        Assert.That(act, Throws.TypeOf<InvalidPasswordException>());
    }

    private async Task registerEmployee(string employee, params Roles[] roles)
    {
        await employeeService.Register(employee, "abcdef123Av#");
        var employeeReadModel = await employeeQueryService.GetByEmail(employee);
        await employeeService.SetRoles(employeeReadModel.Id, roles);
    }
}