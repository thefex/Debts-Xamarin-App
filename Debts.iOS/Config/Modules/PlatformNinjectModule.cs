using Debts.DataAccessLayer;
using Debts.iOS.Services;
using Debts.iOS.Services.AppGrowth;
using Debts.iOS.Services.Notifications;
using Debts.iOS.Services.Payments;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.Auth;
using Debts.Services.Contacts;
using Debts.Services.Notifications;
using Debts.Services.Payments;
using Debts.Services.Phone;
using Debts.Services.Settings;
using Ninject;
using Ninject.Modules;
using ObjCRuntime;

namespace Debts.iOS.Config.Modules
{
    public class PlatformNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<StartService>().ToSelf();
            Bind<IStartService>().To<StartServiceNotificationsPermissionProxy>();
            
            Bind<IStorageService>()
                .To<UserDefaultsStorageService>()
                .InSingletonScope();

            Bind<AppRelatedStatisticsService>()
                .To<iOSAppRelatedStatisticsService>()
                .InSingletonScope();

            Bind<RateAppService>()
                .To<AppStoreRateAppService>();

            Bind<AdvertisementService>()
                .To<iOSAdvertisementService>()
                .InSingletonScope();

            Bind<INotificationsWorkScheduler>()
                .To<iOSNotificationsWorkScheduler>()
                .InSingletonScope();

            Bind<IBillingService>()
                .To<AppStoreBillingService>()
                .InSingletonScope();
            
            Bind<PhoneContactsService>().ToSelf();

            Bind<IPhoneContactsService>()
                .ToMethod(ctx =>
                { 
                    var queryCommandExecutor = ctx.Kernel.Get<QueryCommandExecutor>(); 
                    var contactsService = ctx.Kernel.Get<PhoneContactsService>();
                    
                    return new ImportContactsStoreInDatabaseProxy(contactsService, queryCommandExecutor);
                });

            Bind<PermissionService>()
                .To<iOSPermissionService>();

            Bind<IPhoneCallPermission>()
                .To<PhoneCallPermission>();

            Bind<IDisplayNameProvider>()
                .To<iOSDisplayNameProvider>();

            Bind<ShareLinkBuilderService>()
                .To<FirebaseAppStoreShareLinkBuilderService>();

            Bind<IPlatform>().To<iOSPlatform>().InSingletonScope();
        }
    }
}