using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Debts.Config;
using Debts.iOS.Config.Modules;
using Debts.Messenging;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Platforms.Ios.Core;
using MvvmCross.Platforms.Ios.Presenters;
using MvvmCross.ViewModels;
using Ninject.Modules;
using UIKit;

namespace Debts.iOS.Config
{
    public class AppSetup : MvxIosSetup<iOSApplicationSetup>
    {
        private readonly INinjectModule[] _ninjectModules;

        public AppSetup() : this(new PlatformNinjectModulesProvider().GetNinjectModules())
        {
            
        }
        
        public AppSetup(params INinjectModule[] ninjectModules) 
        {
            _ninjectModules = ninjectModules;
        }

        protected override IMvxIosViewPresenter CreateViewPresenter() => new MvxAppPresenter(ApplicationDelegate as UIApplicationDelegate, Window);

        public NinjectMvvmCrossIoCProvider NinjectMvvmCrossProvider { get; private set; }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            NinjectMvvmCrossProvider.ExecuteDelayedCallback();
            MessageQueueInitialization.Initialize();
        }
        
        public override IEnumerable<Assembly> GetPluginAssemblies()
        {
            var pluginAssemblies = base.GetPluginAssemblies().ToList();
            pluginAssemblies.Remove(typeof(MvvmCross.Plugin.PictureChooser.Platforms.Ios.Plugin).Assembly);
            return pluginAssemblies;
        }

        protected override IMvxIoCProvider CreateIocProvider()
        {
            if (_ninjectModules == null || !_ninjectModules.Any())
                throw new InvalidOperationException(
                    "Application configuration failure. If you don't override CreateIocProvider - then you need to supply NinjectModules in constructor. (ninject is default ioc provider)");

            NinjectMvvmCrossProvider = new NinjectMvvmCrossIoCProvider(_ninjectModules);
            return NinjectMvvmCrossProvider;
        } 
    }

    public class iOSApplicationSetup : ApplicationSetup
    {
        public iOSApplicationSetup()
        {
        }

        protected override IMvxViewModelLocator CreateDefaultViewModelLocator()
        {
            return new AppMvxViewModelLocator();
        }
    }
}