using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using R2S.EmployeeManagement.Api.Models;
using R2S.EmployeeManagement.Core;
using R2S.EmployeeManagement.Core.Enums;
using R2S.EmployeeManagement.Core.Read;
using R2S.EmployeeManagement.Core.Read.Queries;
using R2S.EmployeeManagement.Core.Read.Queries.Results;
using R2S.EmployeeManagement.Core.Read.ReadModels;
using R2S.EmployeeManagement.Core.Services;

namespace R2S.EmployeeManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class UsersController : ControllerBase
    {
        private IEmployeeQueryService _userQueryService;
        private IEmployeeService _userService;

        public UsersController(IEmployeeQueryService userQueryService,
            IEmployeeService userService)
        {
            _userQueryService = userQueryService ?? throw new ArgumentNullException(nameof(userQueryService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [ProducesResponseType(typeof(EmployeeReadModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{userId:Guid}")]
        public async Task<IActionResult> GetAsync(Guid userId)
        {
            var user = await _userQueryService.GetById(userId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }


        [ProducesResponseType(typeof(ListEmployeeQueryResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("list")]
        public async Task<IActionResult> GetUsersAsync([FromQuery] ListEmployeeQuery listUserQuery)
        {
            var result = await _userQueryService.GetUsers(listUserQuery);

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDTO), StatusCodes.Status400BadRequest)]
        [HttpPatch("{userId:Guid}/roles")]
        public async Task<IActionResult> SaveRolesAsync(Guid userId, [FromBody]Roles[] roles)
        {
            var result = await _userService.SaveUserRoles(userId, roles);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiErrorDTO(result.Errors));
            }

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDTO), StatusCodes.Status400BadRequest)]
        [HttpPatch("{userId:Guid}/password")]
        public async Task<IActionResult> SetPassword(Guid userId, [FromBody] string newPassword)
        {
            var result = await _userService.SetPassword(userId, newPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiErrorDTO(result.Errors));
            }

            return Ok();
        }
    }
}
