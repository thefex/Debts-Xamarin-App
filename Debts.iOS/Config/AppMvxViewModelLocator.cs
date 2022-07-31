using System;
using Debts.iOS.Config.ViewModelResolver;
using MvvmCross.Navigation.EventArguments;
using MvvmCross.ViewModels;

namespace Debts.iOS.Config
{
    public class AppMvxViewModelLocator : MvxDefaultViewModelLocator
    {  
        private readonly MvxCustomViewModelResolverContainer _customViewModelResolverContainer =
            new MvxCustomViewModelResolverContainer();

        public AppMvxViewModelLocator()
        {
        }

        public override IMvxViewModel Load(Type viewModelType, IMvxBundle parameterValues, IMvxBundle savedState,
            IMvxNavigateEventArgs navigationArgs)
        {
            if (_customViewModelResolverContainer.HasRegisteredCustomViewModelResolver(viewModelType))
                return _customViewModelResolverContainer.ResolveViewModel(viewModelType);
            
            return base.Load(viewModelType, parameterValues, savedState, navigationArgs);
        }
    }
}