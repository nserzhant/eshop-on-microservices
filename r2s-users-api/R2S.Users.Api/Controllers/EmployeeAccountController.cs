using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using R2S.Users.Api.Models;
using R2S.Users.Api.Settings;
using R2S.Users.Core.Services;
using System.Security.Claims;

namespace R2S.Users.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeAccountController : ControllerBase
    {
        private readonly JWTSettings _jwtSettings;
        private readonly IUserService _userService;

        public EmployeeAccountController(IOptions<JWTSettings> options, IUserService userService)
        {
            _jwtSettings = options.Value ?? throw new ArgumentNullException(nameof(JWTSettings));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDTO), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Regiser(UserDTO userDTO)
        {
            var result = await _userService.Register(userDTO.Email, userDTO.Password);

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
            var userId = getUserId();
            var result = await _userService.ChangePassword(userId, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiErrorDTO(result.Errors));
            }

            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorDTO), StatusCodes.Status400BadRequest)]
        [HttpPatch("changeemail")]
        public async Task<IActionResult> ChangeEmail(UserDTO userDTO)
        {
            var userId = getUserId();
            var result = await _userService.ChangeEmail(userId, userDTO.Email, userDTO.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiErrorDTO(result.Errors));
            }

            return Ok();
        }

        private Guid getUserId()
        {
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Guid.Empty;
            }

            return new Guid(userId);
        }
    }
}
