using Microsoft.AspNetCore.Identity;

namespace EShop.EmployeeManagement.Api.Models;

/// <summary>
/// Response container that contains all types of possible response errors.
/// </summary>
public class ApiErrorDTO
{
    public IdentityErrorsDTO? IdentityErrors { get; private set; }

    public ApiErrorDTO(IEnumerable<IdentityError> identityErrors)
    {
        IdentityErrors = new IdentityErrorsDTO(identityErrors);
    }
}