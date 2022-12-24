using R2S.Users.Core.Exceptions;

namespace R2S.Users.Api.Models
{
    public class UsersDomainErrorDTO
    {
        public string ErrorType { get; private set; }

        public UsersDomainErrorDTO(BaseUsersDomainException applicationException)
        {
            ErrorType = camelize(applicationException.GetType().Name);
        }

        private string camelize(string name)
        {
            var camelized = Char.ToLowerInvariant(name[0]) + name.Substring(1);
            return camelized;
        }
    }
}
