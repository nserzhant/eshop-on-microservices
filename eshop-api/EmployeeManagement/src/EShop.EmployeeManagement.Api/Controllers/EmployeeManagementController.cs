using EShop.EmployeeManagement.Api.Models;
using EShop.EmployeeManagement.Infrastructure.Entities;
using EShop.EmployeeManagement.Infrastructure.Enums;
using EShop.EmployeeManagement.Infrastructure.Read;
using EShop.EmployeeManagement.Infrastructure.Read.Queries;
using EShop.EmployeeManagement.Infrastructure.Read.Queries.Results;
using EShop.EmployeeManagement.Infrastructure.Read.ReadModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EShop.EmployeeManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class EmployeeManagementController : ControllerBase
{
    private readonly IEmployeeQueryService _employeeQueryService;
    private readonly UserManager<Employee> _userManager;
    private readonly ILogger<EmployeeManagementController> _logger;

    public EmployeeManagementController(IEmployeeQueryService employeeQueryService,
        UserManager<Employee> userManager,
        ILogger<EmployeeManagementController> logger)
    {
        _employeeQueryService = employeeQueryService ?? throw new ArgumentNullException(nameof(employeeQueryService));
        _userManager = userManager;
        _logger = logger;
    }

    [ProducesResponseType(typeof(EmployeeReadModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("{employeeId:Guid}")]
    public async Task<IActionResult> GetAsync(Guid employeeId)
    {
        var employee = await _employeeQueryService.GetById(employeeId);

        if (employee == null)
            return NotFound();

        return Ok(employee);
    }

    [ProducesResponseType(typeof(ListEmployeeQueryResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpGet("list")]
    public async Task<IActionResult> GetEmployeesAsync([FromQuery] ListEmployeeQuery listEmployeeQuery)
    {
        var result = await _employeeQueryService.GetEmployees(listEmployeeQuery);

        return Ok(result);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpPatch("{employeeId:Guid}/roles")]
    public async Task<IActionResult> SetRolesAsync(Guid employeeId, [FromBody] Roles[] roles)
    {
        var employee = await _userManager.FindByIdAsync(employeeId.ToString());

        if (employee == null) return NotFound();

        var roleNames = roles.Select(r => r.ToString());
        var existingRoles = await _userManager.GetRolesAsync(employee);
        var result = await _userManager.RemoveFromRolesAsync(employee, existingRoles);

        if (!result.Succeeded)
        {
            _logger.LogError($"Failed to update roles for employeeId: {employeeId}");

            return BadRequest(new ApiErrorDTO(result.Errors));
        }

        result = await _userManager.AddToRolesAsync(employee, roleNames);

        if (!result.Succeeded)
        {
            _logger.LogError($"Failed to update roles for employeeId: {employeeId}");
            return BadRequest(new ApiErrorDTO(result.Errors));
        }

        _logger.LogInformation($"Roles successfully updated for employeeId: {employeeId} ");

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpPatch("{employeeId:Guid}/password")]
    public async Task<IActionResult> SetPassword(Guid employeeId, [FromBody] string newPassword)
    {
        var employee = await _userManager.FindByIdAsync(employeeId.ToString());

        if (employee == null) return NotFound();

        var token = await _userManager.GeneratePasswordResetTokenAsync(employee);
        var result = await _userManager.ResetPasswordAsync(employee, token, newPassword);

        if (!result.Succeeded)
        {
            _logger.LogError($"Failed to update password for employeeId: {employeeId}");

            return BadRequest(new ApiErrorDTO(result.Errors));
        }

        return Ok();
    }
}
