using System.Threading.Tasks;
using Debts.Services;
using Plugin.Permissions.Abstractions;

namespace Debts.iOS.Services
{
    public class iOSPermissionService : PermissionService
    {
        public iOSPermissionService(IStorageService storageService) : base(storageService)
        {
        }

        public override Task<bool> HasPermission(Permission permission)
        {
            if (permission == Permission.Contacts)
                return Task.FromResult(true);
            
            return base.HasPermission(permission);
        }

        public override Task<PermissionStatus> RequestPermissions(Permission permission)
        {
            if (permission == Permission.Contacts)
                return Task.FromResult(PermissionStatus.Granted);
            
            return base.RequestPermissions(permission);
        }
    }
}