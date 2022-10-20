
using Microsoft.AspNetCore.Identity;
using R2S.Users.Core.Enums;
using System.Security.Claims;

namespace R2S.Users.Core.Services
{
    public interface IUserService
    {
        Task<IEnumerable<Claim>> Login(string email, string password);
        Task<IdentityResult> Register(string email, string password);
        Task<IdentityResult> SaveUserRoles(Guid userId, params Roles[] roles);
        Task<IdentityResult> SetPassword(Guid userId, string newPassword);
        Task<IdentityResult> ChangePassword(Guid userId, string oldPassword, string newPassword);
        Task<IdentityResult> ChangeEmail(Guid userId, string newEmail);
    }
}