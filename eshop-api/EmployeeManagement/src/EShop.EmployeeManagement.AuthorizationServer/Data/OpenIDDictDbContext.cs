using Microsoft.EntityFrameworkCore;

namespace EShop.EmployeeManagement.AuthorizationServer.Data;

public class OpenIDDictDbContext : DbContext
{
    public OpenIDDictDbContext(DbContextOptions<OpenIDDictDbContext> options)
        : base(options) { }
}
