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
            var employee = new Employee
            {
                Email = email,
                UserName = email
            };

            var result = await _userManager.CreateAsync(employee, password);

            if (result.Succeeded)
                _logger.LogInformation($"Employee registered");

            return result;
        }

        public async Task<IEnumerable<Claim>> Login(string email, string password)
        {
            var employee = await _userManager.FindByEmailAsync(email);

            if (employee == null)
                throw new InvalidEmailOrPasswordException();

            if (!await _userManager.CheckPasswordAsync(employee, password))
                throw new InvalidEmailOrPasswordException();

            _logger.LogInformation("Employee login credentials checked successfully");

            var existingRoles = await _userManager.GetRolesAsync(employee);
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, employee.UserName));

            foreach (var role in existingRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));


            return claims;
        }

        public async Task<IdentityResult> SetRoles(Guid employeeId, params Roles[] roles)
        {
            var employee = await _userManager.FindByIdAsync(employeeId.ToString());

            if (employee == null)
                throw new EmployeeNotFoundException();

            var roleNames = roles.Select(r => r.ToString());
            var existingRoles = await _userManager.GetRolesAsync(employee);
            var rolesToRemove = existingRoles.Except(roleNames);
            var rolesToAdd = roleNames.Except(existingRoles);

            var result = await _userManager.RemoveFromRolesAsync(employee, rolesToRemove);

            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to update roles for employeeId: {employeeId}");

                return result;
            }

            result = await _userManager.AddToRolesAsync(employee, rolesToAdd);


            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to update roles for employeeId: {employeeId}");
            }
            else
            {
                _logger.LogInformation($"Roles successfully updated for employeeId: {employeeId} ");
            }

            return result;
        }

        public async Task<IdentityResult> SetPassword(Guid employeeId, string newPassword)
        {
            var employee = await _userManager.FindByIdAsync(employeeId.ToString());

            if (employee == null)
                throw new EmployeeNotFoundException();

            var token = await _userManager.GeneratePasswordResetTokenAsync(employee);
            var result = await _userManager.ResetPasswordAsync(employee, token, newPassword);

            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to set password for employeeId: {employeeId}");
            }
            else
            {
                _logger.LogInformation($"Password successfully set for employeeId: {employeeId} ");
            }

            return result;
        }

        public async Task<IdentityResult> ChangePassword(Guid employeeId, string oldPassword, string newPassword)
        {
            var employee = await _userManager.FindByIdAsync(employeeId.ToString());

            if (employee == null)
                throw new EmployeeNotFoundException();

            if (!await _userManager.CheckPasswordAsync(employee, oldPassword))
                throw new InvalidPasswordException();

            var result = await _userManager.ChangePasswordAsync(employee, oldPassword, newPassword);

            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to change password for employeeId: {employeeId}");
            }
            else
            {
                _logger.LogInformation($"Password successfully changed for employeeId: {employeeId} ");
            }

            return result;
        }

        public async Task<IdentityResult> ChangeEmail(Guid employeeId, string newEmail, string password)
        {
            var employee = await _userManager.FindByIdAsync(employeeId.ToString());

            if (employee == null)
                throw new EmployeeNotFoundException();

            if (!await _userManager.CheckPasswordAsync(employee, password))
                throw new InvalidPasswordException();

            employee.UserName = newEmail;
            employee.Email = newEmail;

            var result = await _userManager.UpdateAsync(employee);

            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to change password for employeeId: {employeeId}");
            }
            else
            {
                _logger.LogInformation($"Password successfully changed for employeeId: {employeeId} ");
            }

            return result;
        }
    }
}
