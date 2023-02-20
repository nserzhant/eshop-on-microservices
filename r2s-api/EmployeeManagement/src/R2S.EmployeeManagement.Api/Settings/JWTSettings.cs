namespace R2S.EmployeeManagement.Api.Settings;

public class JWTSettings
{
    public string JWTSecretKey { get; set; } = "";
    public string Audience { get; set; } = "";
    public string Issuer { get; set; } = "";
}
