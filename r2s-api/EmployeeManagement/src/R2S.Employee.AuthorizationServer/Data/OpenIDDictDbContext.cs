using Microsoft.EntityFrameworkCore;

namespace R2S.EmployeeManagement.AuthorizationServer.Data
{
    public class OpenIDDictDbContext : DbContext
    {
        public OpenIDDictDbContext(DbContextOptions<OpenIDDictDbContext> options)
            : base(options) { }
    }
}
