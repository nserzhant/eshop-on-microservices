using Microsoft.AspNetCore.Identity;

namespace EShop.EmployeeManagement.Api.Models;

public class IdentityErrorsDTO
{
    public IEnumerable<IdentityError> Errors { get; private set; }

    public IdentityErrorsDTO(IEnumerable<IdentityError> errors)
    {
        Errors = errors;
    }
}
