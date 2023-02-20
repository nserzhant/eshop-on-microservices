using System.Text.Json.Serialization;

namespace R2S.EmployeeManagement.Core.Read.ReadModels;

public class RoleReadModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    [JsonIgnore]
    public List<EmployeeReadModel> Users { get; set; }
}
