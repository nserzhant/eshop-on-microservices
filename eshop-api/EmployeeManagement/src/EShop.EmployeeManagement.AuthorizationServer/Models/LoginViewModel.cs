using System.ComponentModel.DataAnnotations;

namespace EShop.EmployeeManagement.AuthorizationServer.Models;

public class LoginViewModel
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}
