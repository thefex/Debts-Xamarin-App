using System.Threading.Tasks;
using Debts.Services.Auth;
using UserNotifications;

namespace Debts.iOS.Services
{
    public class StartServiceNotificationsPermissionProxy : IStartService
    {
        private readonly IStartService _startService;

        public StartServiceNotificationsPermissionProxy(StartService startService)
        {
            _startService = startService;
        }
        
        public Task<bool> HasAppEverStarted()
        {
            return _startService.HasAppEverStarted();
        }

        public async Task SetAppAsStarted()
        {
            var response = await UNUserNotificationCenter.Current
                .RequestAuthorizationAsync(UNAuthorizationOptions.ProvidesAppNotificationSettings | 
                                           UNAuthorizationOptions.Alert |
                                           UNAuthorizationOptions.Sound);
            
            await _startService.SetAppAsStarted();
        }
    }
}