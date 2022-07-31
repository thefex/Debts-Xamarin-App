using System.Threading.Tasks;
using Debts.Services.Phone;

namespace Debts.iOS.Services
{
    public class PhoneCallPermission : IPhoneCallPermission
    {
        public Task<bool> RequestPhoneCallStatePermission()
        {
            return Task.FromResult(true);
        }
    }
}