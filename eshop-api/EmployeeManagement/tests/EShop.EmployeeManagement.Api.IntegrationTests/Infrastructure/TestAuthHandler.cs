using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace EShop.EmployeeManagement.Api.IntegrationTests.Infrastructure;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly TestAuthenticationContextBuilder testAuthenticationContextBuilder_;

    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
        TestAuthenticationContextBuilder testAuthenticationContextBuilder)
        : base(options, logger, encoder, clock)
    {
        testAuthenticationContextBuilder_ = testAuthenticationContextBuilder;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!testAuthenticationContextBuilder_.IsAuthenticated)
        {
            var failResult = AuthenticateResult.Fail("Fail");
            return Task.FromResult(failResult);
        }

        var identity = new ClaimsIdentity(testAuthenticationContextBuilder_.Claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");
        var successResult = AuthenticateResult.Success(ticket);

        return Task.FromResult(successResult);
    }
}
