using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EShop.EmployeeManagement.Core;
using EShop.EmployeeManagement.DB.Manager.Seeders;

if (args.Length == 0)
{
    printInformationMessages();
    return;
}

var commandArgument = args[0];

if (commandArgument == "-create-administrator")
{
    if(args.Length < 3)
    {
        printInformationMessages();
        return;
    }

    string adminEmail = args[1];
    string adminPassword = args[2];

    using var services = setupServices();

    var seeder = services.GetRequiredService<CreateAdministratorSeeder>();
    await seeder.Seed(adminEmail, adminPassword);
    return;
}

if (commandArgument == "-update-database")
{
    using var services = setupServices();

    var dbContext = services.GetRequiredService<EmployeeDbContext>();
    dbContext.Database.Migrate();
    return;
}

printInformationMessages();

static ServiceProvider setupServices()
{
    var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    ServiceCollection sc = new ServiceCollection();

    sc.AddEmployeeServices(configuration);
    sc.AddLogging(logging => logging.AddConsole());
    sc.AddTransient<CreateAdministratorSeeder>();

    var serviceProvier = sc.BuildServiceProvider();

    return serviceProvier;
}

static void printInformationMessages()
{
    Console.WriteLine("Provide command line args to execute operation");
    Console.WriteLine("To create administrator execute with command-line arguments: -create-administrator put-administrator-email-here put-administrator-password-here");
    Console.WriteLine("To upadte database execute with command line argument: -update-database");
}