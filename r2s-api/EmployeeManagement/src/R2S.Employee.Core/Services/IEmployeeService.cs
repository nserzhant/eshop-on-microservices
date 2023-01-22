
using Microsoft.AspNetCore.Identity;
using R2S.EmployeeManagement.Core.Enums;
using System.Security.Claims;

namespace R2S.EmployeeManagement.Core.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Claim>> Login(string email, string password);
        Task<IdentityResult> Register(string email, string password);
        Task<IdentityResult> SaveUserRoles(Guid userId, params Roles[] roles);
        Task<IdentityResult> SetPassword(Guid userId, string newPassword);
        Task<IdentityResult> ChangePassword(Guid userId, string oldPassword, string newPassword);
        Task<IdentityResult> ChangeEmail(Guid userId, string newEmail, string password);
    }
}