namespace R2S.Users.Api.Models
{
    public class ChangePasswordDTO
    {
        public string OldPassword { get; set; } = "";

        public string NewPassword { get; set; } = "";
    }
}