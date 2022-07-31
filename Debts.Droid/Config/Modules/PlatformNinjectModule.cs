using Android.Content;
using Debts.Config.Modules;
using Debts.DataAccessLayer;
using Debts.Droid.Services;
using Debts.Droid.Services.AppGrowth;
using Debts.Droid.Services.Notifications;
using Debts.Droid.Services.Payments;
using Debts.Droid.Services.Settings;
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

namespace Debts.Droid.Config.Modules
{
    public class PlatformNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IStartService>().To<StartService>();

            Bind<PhoneContactsService>().ToSelf();

            Bind<IPhoneContactsService>()
                .ToMethod(ctx =>
                { 
                    var queryCommandExecutor = ctx.Kernel.Get<QueryCommandExecutor>(); 
                    var contactsService = ctx.Kernel.Get<PhoneContactsService>();
                    
                    return new ImportContactsStoreInDatabaseProxy(contactsService, queryCommandExecutor);
                });
                
            Bind<IStorageService>().To<SharedPreferencesStorageService>(); 
            Bind<IPhoneCallPermission>().To<PhoneCallPermission>().InSingletonScope();
            Bind<INotificationsWorkScheduler>().To<AndroidNotificationsWorkScheduler>();

            Bind<AppRelatedStatisticsService>()
                .To<AndroidAppRelatedStatisticsService>()
                .InSingletonScope();

            Bind<RateAppService>()
                .To<AndroidRateAppService>();
            
            Bind<AdvertisementService>()
                .To<AdMobAdvertisementService>()
                .InSingletonScope();

            Bind<IBillingService>()
                .To<GoogleBillingClient>()
                .InSingletonScope();

            Bind<ShareLinkBuilderService>()
                .To<FirebaseShareLinkBuilderService>()
                .InSingletonScope();

            Bind<IDisplayNameProvider>()
                .To<AccountManagerDisplayNameProvider>();

            Bind<IPlatform>().To<DroidPlatform>().InSingletonScope();
        }
    }
}