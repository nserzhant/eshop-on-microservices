using Microsoft.EntityFrameworkCore;

namespace EShop.Customer.AuthorizationServer.Data;

public class OpenIDDictDbContext : DbContext
{
    public OpenIDDictDbContext(DbContextOptions<OpenIDDictDbContext> options)
        : base(options) { }
}