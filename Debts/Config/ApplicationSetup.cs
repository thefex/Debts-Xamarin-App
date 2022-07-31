using System;
using System.Reflection;
using System.Threading.Tasks;
using Debts.Services.AppGrowth;
using Debts.Services.Auth;
using Debts.ViewModel;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Debts.Config
{
    public class ApplicationSetup : MvxApplication
    {
        public const string PageLinkDomain = "https://debtsmanager.page.link";
        
        public override void Initialize()
        {
            base.Initialize();

            bool isLoggedIn = false;
             
            Task.Run(async () => isLoggedIn = await Mvx.IoCProvider.Resolve<IStartService>().HasAppEverStarted()).Wait();
             
            if (!isLoggedIn)
               RegisterAppStart<StartViewModel>();
           else
               RegisterAppStart<MainViewModel>(); 
        }
    }
}