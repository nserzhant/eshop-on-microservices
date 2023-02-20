using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using R2S.EmployeeManagement.Api.Models;
using R2S.EmployeeManagement.Api.Settings;
using R2S.EmployeeManagement.Core.Services;
using System.Security.Claims;

namespace R2S.EmployeeManagement.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EmployeeAccountController : ControllerBase
{
    private readonly JWTSettings _jwtSettings;
    private readonly IEmployeeService _employeeService;

    public EmployeeAccountController(IOptions<JWTSettings> options, IEmployeeService employeeService)
    {
        _jwtSettings = options.Value ?? throw new ArgumentNullException(nameof(JWTSettings));
        _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorDTO), StatusCodes.Status400BadRequest)]
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Regiser(EmployeeDTO employeeDTO)
    {
        var result = await _employeeService.Register(employeeDTO.Email, employeeDTO.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new ApiErrorDTO(result.Errors));
        }

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpPatch("changepassword")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
    {
        var employeeId = getEmployeeId();
        var result = await _employeeService.ChangePassword(employeeId, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);

        if (!result.Succeeded)
        {
            return BadRequest(new ApiErrorDTO(result.Errors));
        }

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorDTO), StatusCodes.Status400BadRequest)]
    [HttpPatch("changeemail")]
    public async Task<IActionResult> ChangeEmail(EmployeeDTO employeeDTO)
    {
        var employeeId = getEmployeeId();
        var result = await _employeeService.ChangeEmail(employeeId, employeeDTO.Email, employeeDTO.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new ApiErrorDTO(result.Errors));
        }

        return Ok();
    }

    private Guid getEmployeeId()
    {
        var employeeId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        if (employeeId == null)
        {
            return Guid.Empty;
        }

        return new Guid(employeeId);
    }
}
