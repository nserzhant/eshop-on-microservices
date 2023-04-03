namespace EShop.EmployeeManagement.Api.Models;

public class ChangePasswordDTO
{
    public string? OldPassword { get; set; } = "";

    public string? NewPassword { get; set; } = "";
}