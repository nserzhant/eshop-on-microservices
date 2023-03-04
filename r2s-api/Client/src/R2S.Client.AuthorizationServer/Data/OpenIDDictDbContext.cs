using Microsoft.EntityFrameworkCore;

namespace R2S.Client.AuthorizationServer.Data;

public class OpenIDDictDbContext : DbContext
{
    public OpenIDDictDbContext(DbContextOptions<OpenIDDictDbContext> options)
        : base(options) { }
}