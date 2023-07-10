using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EShop.Client.AuthorizationServer.Data;

public class DbInitializer
{
    const string TEST_USER_NAME = "testuser@example.com";
    const string TEST_USER_PASSWORD = "l85!cD*mY2";
    public static async Task InitializeDbWIthTestData(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
    {
        await applicationDbContext.Database.MigrateAsync();

        if (!applicationDbContext.Users.Any())
        {
            await userManager.CreateAsync(new ApplicationUser()
            {
                Email = TEST_USER_NAME,
                UserName = TEST_USER_NAME
            }, TEST_USER_PASSWORD);
        }

        await applicationDbContext.SaveChangesAsync();
    }
}