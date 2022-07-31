using System.Threading.Tasks;

namespace Debts.Services.Phone
{
    public interface IPhoneCallPermission
    {
        Task<bool> RequestPhoneCallStatePermission();
    }
}