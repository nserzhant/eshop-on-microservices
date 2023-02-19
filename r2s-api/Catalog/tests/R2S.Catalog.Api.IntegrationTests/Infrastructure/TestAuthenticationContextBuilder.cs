using R2S.Catalog.Api.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace R2S.Catalog.Api.IntegrationTests.Infrastructure;
public class TestAuthenticationContextBuilder
{
    public IList<Claim> Claims { get; } = new List<Claim>();
    public bool IsAuthenticated { get; private set; } = false;

    public TestAuthenticationContextBuilder AsSalesManager()
    {
        Claims.Add(new Claim(ClaimTypes.Role, Roles.SALES_MANAGER_ROLE_NAME));
        return this;
    }

    public TestAuthenticationContextBuilder SetUnauthenticated()
    {
        IsAuthenticated = false;
        return this;
    }

    internal TestAuthenticationContextBuilder SetAuthenticated()
    {
        IsAuthenticated = true;
        return this;
    }
}
