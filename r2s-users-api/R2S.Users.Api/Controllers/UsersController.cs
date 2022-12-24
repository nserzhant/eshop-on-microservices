using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using R2S.Users.Api.Models;
using R2S.Users.Core;
using R2S.Users.Core.Enums;
using R2S.Users.Core.Read;
using R2S.Users.Core.Read.Queries;
using R2S.Users.Core.Read.Queries.Results;
using R2S.Users.Core.Read.ReadModels;
using R2S.Users.Core.Services;

namespace R2S.Users.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class UsersController : ControllerBase
    {
        private IUserQueryService _userQueryService;
        private IUserService _userService;

        public UsersController(IUserQueryService userQueryService,
            IUserService userService)
        {
            _userQueryService = userQueryService ?? throw new ArgumentNullException(nameof(userQueryService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [ProducesResponseType(typeof(UserReadModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("{userId:Guid}")]
        public async Task<IActionResult> GetAsync(Guid userId)
        {
            var user = await _userQueryService.GetById(userId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }


        [ProducesResponseType(typeof(ListUserQueryResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("list")]
        public async Task<IActionResult> GetUsersAsync([FromQuery] ListUserQuery listUserQuery)
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
