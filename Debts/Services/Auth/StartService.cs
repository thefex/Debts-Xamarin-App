using System.Threading.Tasks;

namespace Debts.Services.Auth
{
    public class StartService : IStartService
    {
        private readonly IStorageService _storageService;

        public StartService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        private const string HasAppStartedKey = "hasAppStartedKey";
        public Task<bool> HasAppEverStarted()
        {
            return Task.FromResult(_storageService.Get(HasAppStartedKey, false));
        }

        public Task SetAppAsStarted()
        {
            _storageService.Store(HasAppStartedKey, true);
            return Task.FromResult(true);
        }
    }
}