using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using R2S.EmployeeManagement.Api.Models;
using R2S.EmployeeManagement.Core.Enums;
using R2S.EmployeeManagement.Core.Read;
using R2S.EmployeeManagement.Core.Read.Queries;
using R2S.EmployeeManagement.Core.Read.Queries.Results;
using R2S.EmployeeManagement.Core.Read.ReadModels;
using R2S.EmployeeManagement.Core.Services;

namespace R2S.EmployeeManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class EmployeeManagementController : ControllerBase
{
    private IEmployeeQueryService _employeeQueryService;
    private IEmployeeService _employeeService;

    public EmployeeManagementController(IEmployeeQueryService employeeQueryService,
        IEmployeeService employeeService)
    {
        _employeeQueryService = employeeQueryService ?? throw new ArgumentNullException(nameof(employeeQueryService));
        _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
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
    [ProducesResponseType(typeof(ApiErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpPatch("{employeeId:Guid}/roles")]
    public async Task<IActionResult> SetRolesAsync(Guid employeeId, [FromBody] Roles[] roles)
    {
        var result = await _employeeService.SetRoles(employeeId, roles);

        if (!result.Succeeded)
        {
            return BadRequest(new ApiErrorDTO(result.Errors));
        }

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpPatch("{employeeId:Guid}/password")]
    public async Task<IActionResult> SetPassword(Guid employeeId, [FromBody] string newPassword)
    {
        var result = await _employeeService.SetPassword(employeeId, newPassword);

        if (!result.Succeeded)
        {
            return BadRequest(new ApiErrorDTO(result.Errors));
        }

        return Ok();
    }
}
