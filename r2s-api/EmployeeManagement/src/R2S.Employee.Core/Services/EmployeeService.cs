using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using R2S.EmployeeManagement.Core.Entities;
using R2S.EmployeeManagement.Core.Enums;
using R2S.EmployeeManagement.Core.Exceptions;
using System.Security.Claims;

namespace R2S.EmployeeManagement.Core.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ILogger<EmployeeService> _logger;
        private readonly UserManager<Employee> _userManager;

        public EmployeeService(UserManager<Employee> userManager, ILogger<EmployeeService> logger)
        {
            _userManager = userManager;
            _logger= logger;
        }

        public async Task<IdentityResult> Register(string email, string password)
        {
            var user = new Employee
            {
                Email = email,
                UserName = email
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
                _logger.LogInformation($"User registered");

            return result;
        }

        public async Task<IEnumerable<Claim>> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new InvalidUserOrPasswordException();

            if (!await _userManager.CheckPasswordAsync(user, password))
                throw new InvalidUserOrPasswordException();

            _logger.LogInformation("User login credentials checked successfully");

            var existingRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            foreach (var role in existingRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));


            return claims;
        }

        public async Task<IdentityResult> SaveUserRoles(Guid userId, params Roles[] roles)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new UserNotFoundException();

            var roleNames = roles.Select(r => r.ToString());
            var existingRoles = await _userManager.GetRolesAsync(user);
            var rolesToRemove = existingRoles.Except(roleNames);
            var rolesToAdd = roleNames.Except(existingRoles);

            var result = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to update roles for UserId: {userId}");

                return result;
            }

            result = await _userManager.AddToRolesAsync(user, rolesToAdd);


            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to update roles for UserId: {userId}");
            }
            else
            {
                _logger.LogInformation($"Roles successfully updated for UserId: {userId} ");
            }

            return result;
        }

        public async Task<IdentityResult> SetPassword(Guid userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new UserNotFoundException();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to set password for UserId: {userId}");
            }
            else
            {
                _logger.LogInformation($"Password successfully set for UserId: {userId} ");
            }

            return result;
        }

        public async Task<IdentityResult> ChangePassword(Guid userId, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new UserNotFoundException();

            if (!await _userManager.CheckPasswordAsync(user, oldPassword))
                throw new InvalidPasswordException();

            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to change password for UserId: {userId}");
            }
            else
            {
                _logger.LogInformation($"Password successfully changed for UserId: {userId} ");
            }

            return result;
        }

        public async Task<IdentityResult> ChangeEmail(Guid userId, string newEmail, string password)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new UserNotFoundException();

            if (!await _userManager.CheckPasswordAsync(user, password))
                throw new InvalidPasswordException();

            user.UserName = newEmail;
            user.Email = newEmail;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to change password for UserId: {userId}");
            }
            else
            {
                _logger.LogInformation($"Password successfully changed for UserId: {userId} ");
            }

            return result;
        }
    }
}
