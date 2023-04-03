using Microsoft.EntityFrameworkCore;

namespace EShop.Client.AuthorizationServer.Data;

public class OpenIDDictDbContext : DbContext
{
    public OpenIDDictDbContext(DbContextOptions<OpenIDDictDbContext> options)
        : base(options) { }
}