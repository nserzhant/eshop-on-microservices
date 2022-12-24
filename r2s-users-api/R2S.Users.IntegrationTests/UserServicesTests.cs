using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using R2S.Users.Core;
using R2S.Users.Core.Entities;
using R2S.Users.Core.Enums;
using R2S.Users.Core.Exceptions;
using R2S.Users.Core.Read;
using R2S.Users.Core.Read.Queries;
using R2S.Users.Core.Read.ReadModels;
using R2S.Users.Core.Services;
using R2S.Users.Core.IntegrationTests.Infrastructure;

namespace R2S.Users.Core.IntegrationTests
{

    [TestFixture]
    public class UserServicesTests : BaseUsersIntegrationTests
    {
        protected IUserService userService;
        protected UsersReadDbContext readDbContext;
        protected IUserQueryService userQueryService;

        [SetUp]
        public override async Task Setup()
        {
            await base.Setup();

            userService = serviceProvier.GetRequiredService<IUserService>();
            readDbContext = serviceProvier.GetRequiredService<UsersReadDbContext>();
            userQueryService = serviceProvier.GetRequiredService<IUserQueryService>();
        }

        [Test]
        [Category("User Service")]
        [Category("Register")]
        public void When_Register_User_With_Valid_Credentials_Then_User_Should_Be_Created()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";

            Task act() => userService.Register(userEmail, userPassword);

            Assert.That(act, Throws.Nothing);
        }

        [Test]
        [Category("User Service")]
        [Category("Register")]
        public async Task Whent_Trying_To_Register_User_With_The_Same_Email_Correct_Error_Should_Be_Returned()
        {
            var userEmail = "samople@test.com";
            await registerUser(userEmail);

            var result = await  userService.Register(userEmail, "abcdef123Av#");

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors.First().Code, Is.EqualTo(nameof(IdentityErrorDescriber.DuplicateUserName)));
        }

        [Test]
        [Category("User Service")]
        [Category("Login")]
        public async Task When_User_Is_Registered_Then_Login_Will_Be_Successfull()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);

            var claims = await userService.Login(userEmail, userPassword);

            Assert.That(claims, Is.Not.Null);
        }


        [Test]
        [Category("User Service")]
        [Category("Login")]
        public async Task When_Trying_To_Login_With_Invalid_Email_Then_Correct_Exception_Should_Be_Thrown()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);

            Task act() => userService.Login("newemail1@test.com", userPassword);

            Assert.That(act, Throws.TypeOf<InvalidUserOrPasswordException>());
        }


        [Test]
        [Category("User Service")]
        [Category("Login")]
        public async Task When_Trying_To_Login_With_Invalid_Password_Then_Correct_Exception_Should_Be_Thrown()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);

            Task act() => userService.Login(userEmail, "invalidpassword");

            Assert.That(act, Throws.TypeOf<InvalidUserOrPasswordException>());
        }

        [Test]
        [Category("User Query Service")]
        [Category("GetByEmail")]
        public async Task When_User_Exists_Then_It_Could_Be_Requested_By_Email()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);

            UserReadModel user = await userQueryService.GetByEmail(userEmail);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.Email, Is.EqualTo(userEmail));
        }

        [Test]
        [Category("User Query Service")]
        [Category("GetById")]
        public async Task When_User_Exists_Then_It_Could_Be_Requested_By_Id()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);
            var user = await userQueryService.GetByEmail(userEmail);

            var result = await userQueryService.GetById(user.Id);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        [Category("User Service")]
        [Category("SaveUserRoles")]
        public async Task When_Saving_A_Single_Role_For_The_User_Then_User_Will_Contains_Only_This_Role()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);
            var userReadModel = await userQueryService.GetByEmail(userEmail);

            await userService.SaveUserRoles(userReadModel.Id, Roles.Administrator);
            var user = await userQueryService.GetByEmail(userEmail);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.Roles, Is.Not.Null);
            Assert.That(user.Roles.Count, Is.EqualTo(1));
            Assert.That(user.Roles[0].Name, Is.EqualTo(Roles.Administrator.ToString()));
            Assert.That(user.Email, Is.EqualTo(userEmail));
        }

        [Test]
        [Category("User Service")]
        [Category("SaveUserRoles")]
        public async Task When_Saving_Two_Roles_For_The_User_Then_User_Will_Contains_Both_Roles()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);
            var userReadModel = await userQueryService.GetByEmail(userEmail);

            await userService.SaveUserRoles(userReadModel.Id, Roles.Administrator, Roles.SalesManager);
            var user = await userQueryService.GetByEmail(userEmail);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.Roles, Is.Not.Null);
            Assert.That(user.Roles.Count, Is.EqualTo(2));
            Assert.That(user.Roles.Select(r => r.Name), Does.Contain(Roles.Administrator.ToString()));
            Assert.That(user.Roles.Select(r => r.Name), Does.Contain(Roles.SalesManager.ToString()));
        }

        [Test]
        [Category("User Service")]
        [Category("SaveUserRoles")]
        public async Task When_Adding_Additional_Role_For_The_User_Then_User_Will_Contains_Both_Roles()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);
            var userReadModel = await userQueryService.GetByEmail(userEmail);
            await userService.SaveUserRoles(userReadModel.Id, Roles.SalesManager);

            var result = await userService.SaveUserRoles(userReadModel.Id, Roles.Administrator, Roles.SalesManager);
            var user = await userQueryService.GetByEmail(userEmail);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.Roles, Is.Not.Null);
            Assert.That(user.Roles.Count, Is.EqualTo(2));
            Assert.That(user.Roles.Select(r => r.Name), Does.Contain(Roles.Administrator.ToString()));
            Assert.That(user.Roles.Select(r => r.Name), Does.Contain(Roles.SalesManager.ToString()));
        }

        [Test]
        [Category("User Service")]
        [Category("SaveUserRoles")]
        public async Task When_Saving_A_Single_Role_For_The_User_With_Two_Roles_Then_User_Will_Contains_Only_This_Role()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);
            var userReadModel = await userQueryService.GetByEmail(userEmail);
            await userService.SaveUserRoles(userReadModel.Id, Roles.Administrator, Roles.SalesManager);

            await userService.SaveUserRoles(userReadModel.Id, Roles.SalesManager);
            var user = await userQueryService.GetByEmail(userEmail);

            Assert.That(user, Is.Not.Null);
            Assert.That(user.Roles, Is.Not.Null);
            Assert.That(user.Roles.Count, Is.EqualTo(1));
            Assert.That(user.Roles.Select(r => r.Name), Does.Contain(Roles.SalesManager.ToString()));
        }

        [Test]
        [Category("User Service")]
        [Category("SaveUserRoles")]
        public async Task When_Trying_To_Trying_To_Save_Role_For_Unexisted_User_Then_Correct_Exception_Should_Be_Thrown()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);

            Task act() => userService.SaveUserRoles(Guid.Empty, Roles.Administrator, Roles.SalesManager);

            Assert.That(act, Throws.TypeOf<UserNotFoundException>());
        }

        [Test]
        [Category("User Query Service")]
        [Category("GetUsers")]
        public async Task When_Users_Exists_Then_They_Could_Be_Requested_By_List_Query()
        {
            await registerUser("newemail1test.com", Roles.Administrator, Roles.SalesManager);
            await registerUser("newemail3@test.com", Roles.SalesManager);
            await registerUser("newemail0@test.com", Roles.Administrator, Roles.SalesManager);
            await registerUser("newemail5@test.com", Roles.SalesManager);
            await registerUser("newemail2@test.com", Roles.Administrator, Roles.SalesManager);
            var query = new ListUserQuery();
            query.OrderBy = ListUserOrderBy.Email;
            query.OrderByDirection = OrderByDirections.ASC;
            query.PageSize = 2;
            query.PageIndex = 0;

            var resultAsc = await userQueryService.GetUsers(query);
            query.OrderByDirection = OrderByDirections.DESC;
            var resultDesc = await userQueryService.GetUsers(query);

            Assert.That(resultAsc, Is.Not.Null);
            Assert.That(resultAsc.TotalCount, Is.EqualTo(5));
            Assert.That(resultAsc.Users.Count, Is.EqualTo(2));
            Assert.That(resultAsc.Users[0].Email, Is.EqualTo("newemail0@test.com"));
            Assert.That(resultAsc.Users[0].Roles, Is.Not.Null);
            Assert.That(resultAsc.Users[0].Roles.Count, Is.EqualTo(2));

            Assert.That(resultDesc, Is.Not.Null);
            Assert.That(resultDesc.TotalCount, Is.EqualTo(5));
            Assert.That(resultDesc.Users.Count, Is.EqualTo(2));
            Assert.That(resultDesc.Users[0].Email, Is.EqualTo("newemail5@test.com"));
            Assert.That(resultDesc.Users[0].Roles, Is.Not.Null);
            Assert.That(resultDesc.Users[0].Roles.Count, Is.EqualTo(1));
        }


        [Test]
        [Category("User Query Service")]
        [Category("GetUsers")]
        public async Task When_Users_Exists_Then_They_Could_Be_Filtered_By_List_Query()
        {
            await registerUser("newemail0@test.com", Roles.Administrator);
            await registerUser("anotherEmail@test.com", Roles.SalesManager);
            await registerUser("newemail0@test.com", Roles.Administrator);
            await registerUser("filteredEmail1@test.com", Roles.SalesManager);
            await registerUser("newemail1@test.com", Roles.Administrator);
            await registerUser("newemail2@test.com", Roles.SalesManager);
            await registerUser("additionalFilteredEmail@test.com", Roles.Administrator);
            var query = new ListUserQuery();
            query.OrderBy = ListUserOrderBy.Email;
            query.OrderByDirection = OrderByDirections.ASC;
            query.PageSize = 5;
            query.PageIndex = 0;
            query.EmailFilter = "filter";

            var resultAsc = await userQueryService.GetUsers(query);

            Assert.That(resultAsc, Is.Not.Null);
            Assert.That(resultAsc.TotalCount, Is.EqualTo(2));
            Assert.That(resultAsc.Users.Count, Is.EqualTo(2));
            Assert.That(resultAsc.Users[0].Email, Is.EqualTo("additionalFilteredEmail@test.com"));
            Assert.That(resultAsc.Users[1].Email, Is.EqualTo("filteredEmail1@test.com"));
        }

        [Test]
        [Category("User Service")]
        [Category("SetPassword")]
        public async Task When_Password_Was_Set_Then_User_Could_Login_Using_It()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);
            var userReadModel = await userQueryService.GetByEmail(userEmail);
            var newPassword = "abcdef123Av#$3e!";

            await userService.SetPassword(userReadModel.Id, newPassword);
          
            async Task assert() => await userService.Login(userEmail, newPassword);
            Assert.That(assert, Throws.Nothing);
        }

        [Test]
        [Category("User Service")]
        [Category("SetPassword")]
        public void When_Trying_To_Set_Password_For_Non_Existing_User_Then_Exception_Should_Be_Thrown()
        {
            var nonExistingId = Guid.NewGuid();
            var newPassword = "abcdef123Av#$3e!";

            Task act() => userService.SetPassword(nonExistingId, newPassword);

            Assert.That(act, Throws.TypeOf<UserNotFoundException>());
        }

        [Test]
        [Category("User Service")]
        [Category("ChangePassword")]
        public async Task When_Password_Was_Changed_Then_User_Could_Login_Using_It()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);
            var userReadModel = await userQueryService.GetByEmail(userEmail);
            var newPassword = "abcdef123Av#$3e!";

            await userService.ChangePassword(userReadModel.Id, userPassword, newPassword);

            async Task assert() => await userService.Login(userEmail, newPassword);
            Assert.That(assert, Throws.Nothing);
        }

        [Test]
        [Category("User Service")]
        [Category("ChangePassword")]
        public void When_Trying_To_Change_Password_For_Non_Existing_User_Then_Exception_Should_Be_Thrown()
        {
            var nonExistingId = Guid.NewGuid();
            var userPassword = "abcdef123Av#";
            var newPassword = "abcdef123Av#$3e!";

            Task act() => userService.ChangePassword(nonExistingId, userPassword, newPassword);

            Assert.That(act, Throws.TypeOf<UserNotFoundException>());
        }

        [Test]
        [Category("User Service")]
        [Category("ChangePassword")]
        public async Task When_Trying_To_Change_Password_With_Wrong_Old_Password_Then_Exception_Should_Be_Thrown()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);
            var userReadModel = await userQueryService.GetByEmail(userEmail);
            var newPassword = "abcdef123Av#$3e!";

            Task act() => userService.ChangePassword(userReadModel.Id, $"{userPassword}_wrong", newPassword);

            Assert.That(act, Throws.TypeOf<InvalidPasswordException>());
        }

        [Test]
        [Category("User Service")]
        [Category("ChangeEmail")]
        public async Task When_Email_Was_Changed_Then_User_Could_Login_Using_It()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);
            var userReadModel = await userQueryService.GetByEmail(userEmail);
            var newEmail = "newemail_changed@test.com";

            await userService.ChangeEmail(userReadModel.Id, newEmail, userPassword);

            async Task assert() => await userService.Login(newEmail, userPassword);
            Assert.That(assert, Throws.Nothing);
        }

        [Test]
        [Category("User Service")]
        [Category("ChangeEmail")]
        public void When_Trying_To_Change_Email_For_Non_Existing_User_Then_Exception_Should_Be_Thrown()
        {
            var nonExistingId = Guid.NewGuid();
            var newEmail = "newemail_changed@test.com";
            var userPassword = "abcdef123Av#";

            Task act() => userService.ChangeEmail(nonExistingId, newEmail, userPassword);

            Assert.That(act, Throws.TypeOf<UserNotFoundException>());
        }

        [Test]
        [Category("User Service")]
        [Category("ChangeEmail")]
        public async Task When_Trying_To_Change_Email_With_Wrong_Old_Password_Then_Exception_Should_Be_Thrown()
        {
            var userEmail = "newemail@test.com";
            var userPassword = "abcdef123Av#";
            await userService.Register(userEmail, userPassword);
            var userReadModel = await userQueryService.GetByEmail(userEmail);
            var newEmail = "newemail_changed@test.com";

            Task act() => userService.ChangeEmail(userReadModel.Id, newEmail, $"{userPassword}_wrong");

            Assert.That(act, Throws.TypeOf<InvalidPasswordException>());
        }

        private async Task registerUser(string userEmail, params Roles[] roles)
        {
            await userService.Register(userEmail, "abcdef123Av#");
            var userReadModel = await userQueryService.GetByEmail(userEmail);
            await userService.SaveUserRoles(userReadModel.Id, roles);
        }
    }
}