using System.Threading.Tasks;
using Android;
using Android.Content.PM;
using Android.OS;
using Debts.Services.Phone;
using MvvmCross;
using MvvmCross.Platforms.Android;

namespace Debts.Droid.Services
{
    public class PhoneCallPermission : IPhoneCallPermission
    {
        private TaskCompletionSource<bool> _permissionTaskCompletionSource;
        public const int ReadPhoneStatePermission = 1954;
        public async Task<bool> RequestPhoneCallStatePermission()
        {
            if (Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.M)
                return true;
                
            _permissionTaskCompletionSource = new TaskCompletionSource<bool>();
            var activity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
            if (activity.CheckSelfPermission(Manifest.Permission.ReadPhoneState) == Permission.Granted)
                return true;
              
            activity.RequestPermissions(new string[] { Manifest.Permission.ReadPhoneState}, ReadPhoneStatePermission);
            return await _permissionTaskCompletionSource.Task;
        }

        public void OnPermissionGranted() => _permissionTaskCompletionSource?.SetResult(true);

        public void OnPermissionDenied() => _permissionTaskCompletionSource?.SetResult(false);
    }
}