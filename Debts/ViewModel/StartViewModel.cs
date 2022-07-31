using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Debts.Commands;
using Debts.Services;
using Debts.Services.Auth;
using Debts.ViewModel.OnboardViewModel;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Debts.ViewModel
{
    public class StartViewModel : BaseViewModel<string>
    {
        private readonly IStartService _startService;

        public readonly IList<IMvxViewModel> OnboardPagedViewModels = new List<IMvxViewModel>()
        {
            new FirstDebtsOnboardViewModel(),
            new SecondPhoneContactsOnboardViewModel(),
            new ThirdGpsOnboardViewModel(),
            new FourthNotifyDebtorOnboardViewModel(),
            new BudgetOnboardViewModel(),
            new SixMoreOnboardViewModel()
        };

        public StartViewModel(IStartService startService)
        {
            _startService = startService;
        }
        
        public IMvxViewModel GetDefaultViewModel()
        {
            return OnboardPagedViewModels.First();
        }

        public IMvxViewModel GetNextViewModel(IMvxViewModel currentViewModel)
        {
            int currentIndex = OnboardPagedViewModels.IndexOf(currentViewModel);
            if (currentIndex == -1)
                return null;

            if (currentIndex + 1 >= OnboardPagedViewModels.Count)
                return null;

            return OnboardPagedViewModels[currentIndex + 1];
        }

        public IMvxViewModel GetPreviousViewModel(IMvxViewModel currentViewModel)
        {
            int currentIndex = OnboardPagedViewModels.IndexOf(currentViewModel);
            if (currentIndex == -1)
                return null;

            if (currentIndex - 1 < 0)
                return null;

            return OnboardPagedViewModels[currentIndex - 1];
        }
        
        public MvxCommand SignIn => new MvxExceptionGuardedCommand(async () =>
        {
                await _startService.SetAppAsStarted();
                ServicesLocation.NavigationService.Navigate<MainViewModel>();
            });

        public int GetIndexOfPage(IMvxViewModel targetVcViewModel)
        {
            return OnboardPagedViewModels.IndexOf(targetVcViewModel as IMvxViewModel);
        }
    }
}