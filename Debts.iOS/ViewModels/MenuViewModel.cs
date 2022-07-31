using System.Collections.Generic;
using System.Linq;
using Debts.Commands;
using Debts.Data;
using Debts.iOS.ViewControllers.Base;
using Debts.Model;
using Debts.Resources;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.ViewModel;
using Debts.ViewModel.AppGrowth;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace Debts.iOS.ViewModels
{
    public class MenuViewModel : BaseViewModel<SelectedSubPage>
    {
        private readonly PremiumService _premiumService;

        public MenuViewModel(PremiumService premiumService)
        {
            _premiumService = premiumService;
        }

        public PremiumState PremiumState => _premiumService.PremiumState; 
        
        public class MenuItem
        {
            public string Name { get; set; }
            
            public string Icon { get; set; }
            
            public SelectedSubPage PageType { get; set; }
            
            public bool IsSelected { get; set; }
        }

        public class MenuGroup
        {
            public string GroupName { get; set; }
            
            public IList<MenuItem> Items { get; set; }
        }

        public IEnumerable<MenuGroup> Groups { get; } = new List<MenuGroup>()
        {
            new MenuGroup()
            {
                GroupName = TextResources.MainBottomMenuFragment_Menu_FinanceOperations,
                Items = new List<MenuItem>()
                {
                    new MenuItem()
                    {
                        Name = TextResources.MainBottomMenuFragment_Menu_BudgetAll,
                        Icon = "budget",
                        PageType = SelectedSubPage.Budget
                    },
                    new MenuItem()
                    {
                        Name = TextResources.MainBottomMenuFragment_Menu_All,
                        Icon = "finance_operation_all",
                        PageType = SelectedSubPage.All
                    },
                    new MenuItem()
                    {
                        Name = TextResources.MainBottomMenuFragment_Menu_Debts,
                        Icon = "finance_operation_debts",
                        PageType = SelectedSubPage.Debts
                    },
                    new MenuItem()
                    {
                        Name = TextResources.MainBottomMenuFragment_Menu_Loans,
                        Icon = "finance_operation_loans",
                        PageType = SelectedSubPage.Loans
                    },
                    new MenuItem()
                    {
                        Name = TextResources.MainBottomMenuFragment_Menu_Favorites,
                        Icon = "finance_operation_favorites",
                        PageType = SelectedSubPage.FavoritesFinances
                    }
                }
            },
            new MenuGroup()
            {
                GroupName = TextResources.MainBottomMenuFragment_Menu_General,
                Items = new List<MenuItem>()
                {
                    new MenuItem()
                    {
                        Name = TextResources.MainBottomMenuFragment_Menu_Contacts,
                        Icon = "contacts",
                        PageType = SelectedSubPage.Contacts
                    },
                    new MenuItem()
                    {
                        Name = TextResources.MainBottomMenuFragment_Menu_Statistics,
                        Icon = "statistics",
                        PageType = SelectedSubPage.Statistics
                    },
                    new MenuItem()
                    {
                        Name = TextResources.MainBottomMenuFragment_Menu_Settings,
                        Icon = "settings",
                        PageType = SelectedSubPage.Settings
                    }
                }
            }
        };

       public MvxCommand GoPremium => new MvxExceptionGuardedCommand(() =>
        {
            if (PremiumState == PremiumState.PremiumOwnership)
                return;

            ServicesLocation.NavigationService.Navigate<GoPremiumViewModel>();
        });

       public int AmountOfDaysOfValidPremiumSubscription => _premiumService.GetAmountOfDaysOfValidPremiumSubscription();

       public override void Prepare(SelectedSubPage parameter)
        {
            var selectedMenuItem = Groups.SelectMany(x => x.Items).FirstOrDefault(x => x.PageType == parameter);
            if (selectedMenuItem != null)
                selectedMenuItem.IsSelected = true;
        }
    }
}