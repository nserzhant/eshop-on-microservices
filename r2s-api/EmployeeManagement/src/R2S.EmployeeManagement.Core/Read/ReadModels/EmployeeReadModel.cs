namespace R2S.EmployeeManagement.Core.Read.ReadModels;

public class EmployeeReadModel
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<RoleReadModel> Roles { get; set; }
}
