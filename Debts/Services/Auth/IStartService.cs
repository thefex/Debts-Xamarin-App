using System.Threading.Tasks;

namespace Debts.Services.Auth
{
    public interface IStartService
    {
        Task<bool> HasAppEverStarted();

        Task SetAppAsStarted();
    }
}