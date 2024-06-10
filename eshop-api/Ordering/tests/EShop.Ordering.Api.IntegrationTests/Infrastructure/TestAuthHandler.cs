using System.Security.Claims;

namespace EShop.Ordering.Api.IntegrationTests.Infrastructure;
public class TestAuthenticationContextBuilder
{
    public IList<Claim> Claims { get; } = new List<Claim>();
    public bool IsAuthenticated { get; private set; } = true;

    public void SetUnauthenticated()
    {
        IsAuthenticated = false;
    }

    internal void SetAuthorizedAs(Guid customerId)
    {
        Claims.Add(new Claim(ClaimTypes.NameIdentifier, customerId.ToString()));
    }
}
