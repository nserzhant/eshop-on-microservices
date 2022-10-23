using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using R2S.Users.Api.Models;
using R2S.Users.Api.Settings;
using R2S.Users.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

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

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Regiser(UserDTO userDTO)
        {
            await _userService.Register(userDTO.Email, userDTO.Password);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDTO user)
        {
            var claims = await _userService.Login(user.Email, user.Password);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.JWTSecretKey));
            var token = new JwtSecurityToken(issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            
            return Ok(tokenStr);
        }

        [HttpPatch("changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePasswordDTO)
        {
            var userId = getUserId();
            await _userService.ChangePassword(userId, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);

            return Ok();
        }

        [HttpPatch("changeemail")]
        public async Task<IActionResult> ChangeEmail([FromBody] string newEmail)
        {
            var userId = getUserId();
            await _userService.ChangeEmail(userId, newEmail);

            return Ok();
        }

        private Guid getUserId()
        {
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;

            if(userId == null)
            {
                return Guid.Empty;
            }

            return new Guid(userId);
        }
    }
}
