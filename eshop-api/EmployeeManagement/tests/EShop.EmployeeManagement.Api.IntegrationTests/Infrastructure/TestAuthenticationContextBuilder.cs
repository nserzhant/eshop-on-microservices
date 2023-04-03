using EShop.EmployeeManagement.Core.Enums;
using System.Security.Claims;

namespace EShop.EmployeeManagement.Api.IntegrationTests.Infrastructure;

public class TestAuthenticationContextBuilder
{
    public IList<Claim> Claims { get; } = new List<Claim>();
    public bool IsAuthenticated { get; private set; } = true;

    public TestAuthenticationContextBuilder WithRole(Roles role)
    {
        Claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        return this;
    }

    public void SetUnauthenticated()
    {
        IsAuthenticated = false;
    }

    internal void SetAuthorizedAs(Guid defaultemployeeId)
    {
        Claims.Add(new Claim(ClaimTypes.NameIdentifier, defaultemployeeId.ToString()));
    }
}
