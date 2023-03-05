using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace R2S.Client.AuthorizationServer.Models;

public class LoginViewModel
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
    public List<AuthenticationScheme>? ExternalLogins { get; internal set; }
}
