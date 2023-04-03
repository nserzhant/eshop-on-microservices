using Microsoft.AspNetCore.Identity;
using EShop.EmployeeManagement.Core.Exceptions;

namespace EShop.EmployeeManagement.Api.Models;

/// <summary>
/// Response container that contains all types of possible response errors.
/// </summary>
public class ApiErrorDTO
{
    public IdentityErrorsDTO? IdentityErrors { get; private set; }
    public EmployeeDomainErrorDTO? DomainError { get; private set; }

    public ApiErrorDTO(IEnumerable<IdentityError> identityErrors)
    {
        IdentityErrors = new IdentityErrorsDTO(identityErrors);
    }

    public ApiErrorDTO(BaseEmployeeDomainException applicationException)
    {
        DomainError = new EmployeeDomainErrorDTO(applicationException);
    }
}
