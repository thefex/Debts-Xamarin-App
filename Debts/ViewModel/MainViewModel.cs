using System.Linq;
using System.Threading.Tasks;
using Debts.Commands;
using Debts.Data;
using Debts.Messenging.Messages.App;
using Debts.Model;
using Debts.Model.NavigationData;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.Notifications;
using Debts.Services.Settings;
using Debts.ViewModel.AppGrowth;
using Debts.ViewModel.Budget;
using Debts.ViewModel.Contacts;
using Debts.ViewModel.Finances;
using Debts.ViewModel.FinancesViewModel;
using Debts.ViewModel.Statistics;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Commands;

namespace Debts.ViewModel
{
    public class MainViewModel : BaseViewModel<string>
    {
        private readonly INotificationsWorkScheduler _notificationsWorkScheduler;
        private readonly RateAppService _rateAppService;
        private readonly PremiumService _premiumService;
        private readonly WalkthroughService _walkthroughService;

        public MainViewModel(INotificationsWorkScheduler notificationsWorkScheduler,
            RateAppService rateAppService,
            PremiumService premiumService,
            WalkthroughService walkthroughService)
        {
            _notificationsWorkScheduler = notificationsWorkScheduler;
            _rateAppService = rateAppService;
            _premiumService = premiumService;
            _walkthroughService = walkthroughService;
        }
        
        public MvxCommand AllFinances => new MvxExceptionGuardedCommand(() =>
            {
                ServicesLocation.NavigationService.Navigate<AllFinancesViewModel>();
            });
        
        public MvxCommand Debts => new MvxExceptionGuardedCommand(() =>
        {
            ServicesLocation.NavigationService.Navigate<MyDebtsViewModel>();
        });
        
        public MvxCommand Loans => new MvxExceptionGuardedCommand(() =>
        {
            ServicesLocation.NavigationService.Navigate<MyLoansViewModel>();
        });

        public MvxCommand FavoritesFinances => new MvxExceptionGuardedCommand(() =>
            {
                if (!_premiumService.HasPremium)
                {
                    ServicesLocation.NavigationService.Navigate<GoPremiumViewModel>();
                    return;
                }
                
                ServicesLocation.NavigationService.Navigate<FavoritesFinancesViewModel>();
            });
        
        public MvxCommand Budget => new MvxExceptionGuardedCommand(() =>
        {
            ServicesLocation.NavigationService.Navigate<BudgetListViewModel>();  
        }); 
        
        public MvxCommand GoPremium => new MvxExceptionGuardedCommand(() =>
        {
            if (PremiumState == PremiumState.PremiumOwnership)
                return;

            ServicesLocation.NavigationService.Navigate<GoPremiumViewModel>();
        });

        public MvxCommand Contacts => new MvxExceptionGuardedCommand(() =>
        {
            ServicesLocation.NavigationService.Navigate<ContactListViewModel>();
        });
        
        public MvxCommand Statistics => new MvxExceptionGuardedCommand(() =>
        {
            ServicesLocation.NavigationService.Navigate<StatisticsViewModel>();
        });
        
        public MvxCommand Settings => new MvxExceptionGuardedCommand(() =>
        {
            ServicesLocation.NavigationService.Navigate<SettingsViewModel>();
        });
        
        public MvxCommand Add => new MvxExceptionGuardedCommand(() =>
        {
            if (IsFinanceListOpened())
                AddOperation.Execute();
            
            if (IsContactListOpened())
                AddContact.Execute();
            
            if (IsBudgetListOpened())
                AddBudget.Execute();
            
            if (CurrentlyActiveSubPage == SelectedSubPage.FinanceDetails)
                ServicesLocation.Messenger.Publish(new RequestFinalizeFinanceOperationMessage (this));
        });
        
        public override async Task Initialize()
        {
            _notificationsWorkScheduler.StartRepeatedBackgroundJobs();
            await base.Initialize();

            var initializeTask = _premiumService.Initialize();
            Mvx.IoCProvider.Resolve<IMvxMainThreadAsyncDispatcher>().ExecuteOnMainThreadAsync(async () =>
            {
                await initializeTask;
                Task.Delay(925).ContinueWith(async (x) =>
                {
                    await _premiumService.Initialize();

                    if (_walkthroughService.GetWalkthroughTypesToShowForMainView().Any())
                        return;
                    
                    if (_rateAppService.TryShowAppRatePrompt())
                        return;

                    if (_premiumService.ShouldShowGoPremiumScreen)
                        await ServicesLocation.NavigationService.Navigate<GoPremiumViewModel>();
                });
            });
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            _premiumService.UpdateState();
        }

        public PremiumState PremiumState => _premiumService.PremiumState;

        public int AmountOfDaysOfValidPremiumSubscription => _premiumService.GetAmountOfDaysOfValidPremiumSubscription();

        private bool IsFinanceListOpened()
        {
            return CurrentlyActiveSubPage == SelectedSubPage.All ||
                   CurrentlyActiveSubPage == SelectedSubPage.Debts ||
                   CurrentlyActiveSubPage == SelectedSubPage.Loans ||
                   CurrentlyActiveSubPage == SelectedSubPage.FavoritesFinances;
        }

        private bool IsContactListOpened()
        {
            return CurrentlyActiveSubPage == SelectedSubPage.Contacts;
        }
        
        private bool IsBudgetListOpened()
        {
            return CurrentlyActiveSubPage == SelectedSubPage.Budget;
        }
        
        public MvxCommand AddOperation => new MvxExceptionGuardedCommand(() =>
        {
            var navigationData = new AddFinanceOperationNavigationData()
            {
                Type = SelectedSubPage == SelectedSubPage.Loans ? FinanceOperationType.Loan : FinanceOperationType.Debt
            };
            ServicesLocation.NavigationService.Navigate<AddFinanceOperationViewModel, AddFinanceOperationNavigationData>(navigationData);
        });
        
        public MvxCommand AddBudget => new MvxExceptionGuardedCommand(() =>
        {
            var navigationData = new AddBudgetItemNavigationData()
            {
                Type = BudgetType.Expense
            };
            ServicesLocation.NavigationService.Navigate<AddBudgetViewModel, AddBudgetItemNavigationData>(navigationData);
        });
        
        public MvxCommand AddContact => new MvxExceptionGuardedCommand(() =>
            {
                ServicesLocation.NavigationService.Navigate<AddContactViewModel>();
            });
        
        public SelectedSubPage SelectedSubPage { get; set; }

        SelectedSubPage _currentlyActiveSubPage;
        public SelectedSubPage CurrentlyActiveSubPage
        {
            get => _currentlyActiveSubPage;
            set
            {
                _currentlyActiveSubPage = value;
                if (_currentlyActiveSubPage != SelectedSubPage.AddContact &&
                    _currentlyActiveSubPage != SelectedSubPage.AddOperation && 
                    _currentlyActiveSubPage != SelectedSubPage.AddBudgetItem &&
                    _currentlyActiveSubPage != SelectedSubPage.ContactDetails &&
                    _currentlyActiveSubPage != SelectedSubPage.FinanceDetails)
                    SelectedSubPage = _currentlyActiveSubPage;
            }
        }
    }
}