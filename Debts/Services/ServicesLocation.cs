using Debts.Messenging;
using Debts.Services.Exceptions;
using Debts.ViewModel;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;

namespace Debts.Services
{
    public class ServicesLocation
    {
        public static IMvxNavigationService NavigationService => Mvx.IoCProvider.Resolve<IMvxNavigationService>();
        
        public static ExceptionGuard ExceptionGuard => Mvx.IoCProvider.Resolve<ExceptionGuard>();
        
        public static MessageQueue MessageQueue => Mvx.IoCProvider.Resolve<MessageQueue>();
        
        public static IMvxMessenger Messenger => Mvx.IoCProvider.Resolve<IMvxMessenger>();

        public static bool IsIoSPlatform => Mvx.IoCProvider.Resolve<IPlatform>().IsIos();

        public static bool IsAndroidPlatform => Mvx.IoCProvider.Resolve<IPlatform>().IsAndroid();
    }
}