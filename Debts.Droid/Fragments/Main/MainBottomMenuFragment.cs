using System;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Debts.Data;
using Debts.Model;
using Debts.Resources;
using Debts.ViewModel;
using MvvmCross.Droid.Support.Design;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views.Fragments;

namespace Debts.Droid.Fragments.Main
{
    [MvxFragmentPresentation]
    public class MainBottomMenuFragment : MvxBottomSheetDialogFragment
    {
        public MainBottomMenuFragment()
        {
        }

        public MainBottomMenuFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

           
            var view = this.BindingInflate(Resource.Layout.fragment_navigation_menu, container, false);
            view.ViewTreeObserver.GlobalLayout += (e, a) =>
            {
                BottomSheetDialog dialog = Dialog as BottomSheetDialog;
                var bottomSheet = dialog.FindViewById(Resource.Id.design_bottom_sheet);
                var behavior = BottomSheetBehavior.From(bottomSheet);
                behavior.State = BottomSheetBehavior.StateExpanded; 

            };
            var viewModel = ViewModel as MainViewModel;

            var navigationView = view.FindViewById<NavigationView>(Resource.Id.navigation_view);
            navigationView.GetHeaderView(0).Click += (e, a) =>
            {
                if (viewModel.PremiumState != PremiumState.PremiumOwnership)
                {
                    viewModel.GoPremium.Execute();
                    Dismiss();
                }
            };
            int menuItemToCheckResourceId = 0;
            
            switch (viewModel.SelectedSubPage)
            {
                case SelectedSubPage.All:
                    menuItemToCheckResourceId = Resource.Id.nav_all;
                    break;
                case SelectedSubPage.Debts:
                    menuItemToCheckResourceId = Resource.Id.nav_debts;
                    break;
                case SelectedSubPage.Loans:
                    menuItemToCheckResourceId = Resource.Id.nav_loans;
                    break;
                case SelectedSubPage.FavoritesFinances:
                    menuItemToCheckResourceId = Resource.Id.nav_favorites;
                    break;
                case SelectedSubPage.Contacts:
                    menuItemToCheckResourceId = Resource.Id.nav_contacts;
                    break;
                case SelectedSubPage.Settings:
                    menuItemToCheckResourceId = Resource.Id.nav_settings;
                    break;
                case SelectedSubPage.Statistics:
                    menuItemToCheckResourceId = Resource.Id.nav_statistics;
                    break;
                case SelectedSubPage.Budget:
                    menuItemToCheckResourceId = Resource.Id.nav_budget;
                    break;
            }

            navigationView.Menu.FindItem(menuItemToCheckResourceId).SetChecked(true);

            var menu = navigationView.Menu;
            menu.FindItem(Resource.Id.finance_operations_section)
                .SetTitle(TextResources.MainBottomMenuFragment_Menu_FinanceOperations);
            
            menu.FindItem(Resource.Id.nav_all)
                .SetTitle(TextResources.MainBottomMenuFragment_Menu_All);

            menu.FindItem(Resource.Id.nav_debts)
                .SetTitle(TextResources.MainBottomMenuFragment_Menu_Debts);

            menu.FindItem(Resource.Id.nav_loans)
                .SetTitle(TextResources.MainBottomMenuFragment_Menu_Loans);

            menu.FindItem(Resource.Id.nav_favorites)
                .SetTitle(TextResources.MainBottomMenuFragment_Menu_Favorites);

            menu.FindItem(Resource.Id.general_section)
                .SetTitle(TextResources.MainBottomMenuFragment_Menu_General);

            menu.FindItem(Resource.Id.nav_contacts)
                .SetTitle(TextResources.MainBottomMenuFragment_Menu_Contacts);

            menu.FindItem(Resource.Id.nav_statistics)
                .SetTitle(TextResources.MainBottomMenuFragment_Menu_Statistics);

            menu.FindItem(Resource.Id.nav_settings)
                .SetTitle(TextResources.MainBottomMenuFragment_Menu_Settings);

            menu.FindItem(Resource.Id.nav_budget)
                .SetTitle(TextResources.MainBottomMenuFragment_Menu_BudgetAll); 
             
            navigationView.NavigationItemSelected += (e, a) =>
            {
                
                switch (a.MenuItem.ItemId)
                {
                    case Resource.Id.nav_all:
                        viewModel.AllFinances.Execute();
                        break;
                    case Resource.Id.nav_debts:
                        viewModel.Debts.Execute();
                        break;
                    case Resource.Id.nav_loans:
                        viewModel.Loans.Execute();
                        break; 
                    case Resource.Id.nav_favorites:
                        viewModel.FavoritesFinances.Execute();
                        break;
                    case Resource.Id.nav_statistics:
                        viewModel.Statistics.Execute();
                        break;
                    case Resource.Id.nav_budget:
                        viewModel.Budget.Execute();
                        break;  
                    case Resource.Id.nav_contacts:
                        viewModel.Contacts.Execute();
                        break;
                    case Resource.Id.nav_settings:
                        viewModel.Settings.Execute();
                        break;                        
                }
                Dismiss();
            };

            return view;
        }

        public override int Theme => Resource.Style.Widget_AppTheme_BottomSheet;
    }
}