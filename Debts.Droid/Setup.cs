using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Android.Content;
using Android.Views;
using Android.Widget;
using Debts.Config;
using Debts.Droid.Config;
using Debts.Droid.Config.Bindings;
using Debts.Droid.Config.Calligraphy;
using Debts.Droid.Config.Modules;
using Debts.Messenging;
using Debts.Services;
using MvvmCross;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.Converters;
using MvvmCross.Core;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Exceptions;
using MvvmCross.IoC;
using MvvmCross.Localization;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Plugin.Location.Platforms.Android;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using Ninject.Modules;

namespace Debts.Droid
{
    public class Setup : MvxAppCompatSetup
    {
        private readonly INinjectModule[] _dependencyInjectionModules;

        /// <summary>
        ///     Use only if you're overriding CreateIocProvider and use IoC provider other than default (Ninject).
        /// </summary>
        /// <param name="applicationContext"></param>
        public Setup()
            : this(new DroidNinjectModulesProvider().GetNinjectModules())
        {
        }

        public Setup(params INinjectModule[] ninjectModules)
            : base()
        {
            _dependencyInjectionModules = ninjectModules;
        }

        protected override IEnumerable<Assembly> ValueConverterAssemblies
        {
            get
            {
                var converters = base.ValueConverterAssemblies.ToList();
                //TODO uncomment code providing right ViewModel
                //converters.Add(typeof(MainViewModel).Assembly);
                return converters.Distinct();
            }
        }

        public override IEnumerable<Assembly> GetViewModelAssemblies()
        {
            var assemblies = new HashSet<Assembly>(base.GetViewModelAssemblies());
            assemblies.Add(typeof(ApplicationSetup).Assembly);
            assemblies.Add(typeof(Setup).Assembly);
            return assemblies;
        }

        protected override IMvxApplication CreateApp()
        {
            return new DroidApplicationSetup();
        }

        protected override MvxBindingBuilder CreateBindingBuilder()
        {
            return new CalligraphyMvxAndroidBindingBuilder();
        }

        protected NinjectMvvmCrossIoCProvider NinjectMvvmCrossProvider { get; set; } 
        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();
            
            Mvx.IoCProvider.RegisterType<Context>(() => ApplicationContext);
            NinjectMvvmCrossProvider.ExecuteDelayedCallback();

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-us");


            MessageQueueInitialization.Initialize();
            
            Calligraphy.CalligraphyConfig.InitDefault(new Calligraphy.CalligraphyConfig.Builder()
                .SetDefaultFontPath("fonts/roboto_regular.ttf")
                .SetFontAttrId(Resource.Attribute.fontPath)
                .DisablePrivateFactoryInjection()
                .Build());
        }

        protected override void FillValueConverters(IMvxValueConverterRegistry registry)
        {
            base.FillValueConverters(registry);
            registry.AddOrOverwrite("Language", new MvxLanguageConverter());
        }

        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            MvxAppCompatSetupHelper.FillTargetFactories(registry);
            base.FillTargetFactories(registry);
            
            registry.RegisterCustomBindingFactory<View>("BackgroundDrawableId", x => new MvxViewBackgroundDrawableIdBinding(x));
            registry.RegisterCustomBindingFactory<object>("IsLoading", x => new MvxCustomItemLoadingBinding(x));
            registry.RegisterCustomBindingFactory<object>("ViewVisibility", view => new VisibilityFadeInOutAndroidBinding(view));
            registry.RegisterCustomBindingFactory<ImageView>("BackgroundBubble", view => new MvxViewBackgroundWithChangedColorBinding(view));
        }

        protected override IMvxIoCProvider CreateIocProvider()
        {
            if ((_dependencyInjectionModules == null) || !_dependencyInjectionModules.Any())
                throw new InvalidOperationException(
                    "Application configuration failure. If you don't override CreateIocProvider - then you need to supply NinjectModules in constructor. (ninject is default ioc provider)");

            NinjectMvvmCrossProvider = new NinjectMvvmCrossIoCProvider(_dependencyInjectionModules);
            return NinjectMvvmCrossProvider;
        }

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            var mvxFragmentsPresenter = new MvxAppPresenter(AndroidViewAssemblies);

            return mvxFragmentsPresenter;
        }
        
        public override IEnumerable<Assembly> GetPluginAssemblies()
        {
            var pluginAssemblies = base.GetPluginAssemblies().ToList();
            pluginAssemblies.Remove(typeof(MvxAndroidLocationWatcher).Assembly);
            return pluginAssemblies;
        }

    }

    public class DroidApplicationSetup : ApplicationSetup
    {
        public DroidApplicationSetup()
        {
        }
        
        protected override IMvxViewModelLocator CreateDefaultViewModelLocator() => new AppMvxViewModelLocator(); 
    }
}