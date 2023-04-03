using EShop.Catalog.Api.Constants;
using System.Security.Claims;

namespace EShop.Catalog.Api.IntegrationTests.Infrastructure;
public class TestAuthenticationContextBuilder
{
    public IList<Claim> Claims { get; } = new List<Claim>();
    public bool IsAuthenticated { get; private set; } = false;
    public string AuthenticationScheme { get; private set; } = "Test";

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

    internal TestAuthenticationContextBuilder SetAuthenticated(string authenticationScheme)
    {
        AuthenticationScheme = authenticationScheme;
        IsAuthenticated = true;
        return this;
    }
}
