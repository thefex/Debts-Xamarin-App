using System;
using System.IO;
using Debts.DataAccessLayer;
using Debts.Resources;
using Debts.Services.AppGrowth;
using Debts.Services.Auth;
using Debts.Services.Contacts;
using Debts.Services.LocationService;
using Debts.Services.Phone;
using Debts.Services.Settings;
using MvvmCross.Localization;
using MvvmCross.Plugin.ResxLocalization;
using Ninject;
using Ninject.Modules;
using SQLite;

namespace Debts.Config.Modules
{
    public class PortableServicesNinjectModule : NinjectModule
    {
        public const string PhoneContactsServiceName = "PhoneContactsServiceName";

        public override void Load()
        {
            Bind<QueryCommandExecutor>().ToMethod((ctx) =>
            {
                var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "app_database.db");

                return new QueryCommandExecutor(new SQLiteAsyncConnection(databasePath));
            });

            Bind<PhoneCallServices>().ToSelf().InSingletonScope();
            Bind<CurrencyProvider>().ToSelf().InSingletonScope();
            Bind<LocationService>().ToSelf().InSingletonScope();
            
            Bind<IMvxTextProvider>().ToMethod(ctx => new MvxResxTextProvider(TextResources.ResourceManager)).InSingletonScope();
        }
    }
}