using System.Security.Claims;

namespace EShop.Basket.Api.IntegrationTests.Infrastructure;
public class TestAuthenticationContextBuilder
{
    public IList<Claim> Claims { get; } = new List<Claim>();
    public bool IsAuthenticated { get; private set; } = true;

    public void SetUnauthenticated()
    {
        IsAuthenticated = false;
    }

    public void SetAuthorizedAs(Guid customerId, string email = "sample@email.com")
    {
        Claims.Add(new Claim(ClaimTypes.NameIdentifier, customerId.ToString()));
        Claims.Add(new Claim(ClaimTypes.Email, email));
    }
}
