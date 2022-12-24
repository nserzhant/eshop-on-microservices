using R2S.Users.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace R2S.Users.Api.IntegrationTests.Infrastructure
{
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

        internal void SetAuthorizedAs(Guid defaultUserId)
        {
            Claims.Add(new Claim(ClaimTypes.NameIdentifier, defaultUserId.ToString()));
        }
    }
}
