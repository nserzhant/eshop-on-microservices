using Microsoft.AspNetCore.Identity;
using R2S.Users.Core.Exceptions;

namespace R2S.Users.Api.Models
{
    /// <summary>
    /// Response container that contains all types of possible response errors.
    /// </summary>
    public class ApiErrorDTO
    {
        public IdentityErrorsDTO? IdentityErrors { get; private set; }
        public UsersDomainErrorDTO? DomainError { get; private set; }

        public ApiErrorDTO(IEnumerable<IdentityError> identityErrors)
        {
            IdentityErrors = new IdentityErrorsDTO(identityErrors);
        }

        public ApiErrorDTO(BaseUsersDomainException applicationException)
        {
            DomainError = new UsersDomainErrorDTO(applicationException);
        }
    }
}
