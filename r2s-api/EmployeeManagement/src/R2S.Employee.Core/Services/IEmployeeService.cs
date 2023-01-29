
using Microsoft.AspNetCore.Identity;
using R2S.EmployeeManagement.Core.Enums;
using System.Security.Claims;

namespace R2S.EmployeeManagement.Core.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Claim>> Login(string email, string password);
        Task<IdentityResult> Register(string email, string password);
        Task<IdentityResult> SetRoles(Guid employeeId, params Roles[] roles);
        Task<IdentityResult> SetPassword(Guid employeeId, string newPassword);
        Task<IdentityResult> ChangePassword(Guid employeeId, string oldPassword, string newPassword);
        Task<IdentityResult> ChangeEmail(Guid employeeId, string newEmail, string password);
    }
}