using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace R2S.Catalog.Api.IntegrationTests.Infrastructure;
public class TestAuthenticationSchemeProvider : AuthenticationSchemeProvider
{
    private readonly TestAuthenticationContextBuilder _testAuthenticationContextBuilder;
    public TestAuthenticationSchemeProvider(IOptions<AuthenticationOptions> options , TestAuthenticationContextBuilder testAuthenticationContextBuilder) :
        base(options)
    {
        _testAuthenticationContextBuilder = testAuthenticationContextBuilder;
    }

    public override Task<AuthenticationScheme?> GetSchemeAsync(string name)
    {
        if (name == _testAuthenticationContextBuilder.AuthenticationScheme)
        {
            var scheme = new AuthenticationScheme(
                _testAuthenticationContextBuilder.AuthenticationScheme,
                _testAuthenticationContextBuilder.AuthenticationScheme,
                typeof(TestAuthHandler)
            );

            return Task.FromResult(scheme);
        }

        return base.GetSchemeAsync(name);
    }
}
