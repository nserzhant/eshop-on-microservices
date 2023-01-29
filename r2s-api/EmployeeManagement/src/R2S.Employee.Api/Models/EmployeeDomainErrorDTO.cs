using R2S.EmployeeManagement.Core.Exceptions;

namespace R2S.EmployeeManagement.Api.Models
{
    public class EmployeeDomainErrorDTO
    {
        public string ErrorType { get; private set; }

        public EmployeeDomainErrorDTO(BaseEmployeeDomainException applicationException)
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
