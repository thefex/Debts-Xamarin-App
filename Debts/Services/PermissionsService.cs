using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Debts.Services
{
    public class PermissionService
    {
        private readonly IStorageService _storageService;
        private const string HasPermissionBeenDeniedInPastKey = "HasPermissionBeenDeniedInPast";

        public PermissionService(IStorageService storageService)
        {
            _storageService = storageService;
        }
        
        public virtual async Task<bool> HasPermission(Permission permission)
        {
            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);

            return permissionStatus == PermissionStatus.Granted;
        }

        public virtual async Task<PermissionStatus> RequestPermissions(Permission permission)
        {
            var permissionGrantResponse = await CrossPermissions.Current.RequestPermissionsAsync(permission);
            if (!permissionGrantResponse.ContainsKey(permission))
                return PermissionStatus.Unknown;

            if (permissionGrantResponse[permission] == PermissionStatus.Denied)
                _storageService.Store(GetPermissionKey(permission), true);

            if (permissionGrantResponse[permission] == PermissionStatus.Granted)
                _storageService.Store(GetPermissionKey(permission), false);

            return permissionGrantResponse[permission];
        }

        private string GetPermissionKey(Permission permission)
            => HasPermissionBeenDeniedInPastKey + permission.ToString();

        public Task<bool> IsPermissionDeniedInPast(Permission permission) => Task.FromResult(_storageService.Get(GetPermissionKey(permission), false));
    }
}