using System;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Views;
using Debts.Droid.Activities;
using Debts.ViewModel;
using Debts.ViewModel.Budget;
using Debts.ViewModel.Contacts;
using Debts.ViewModel.Finances;
using MvvmCross;
using MvvmCross.Platforms.Android;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.ViewModels;

namespace Debts.Droid.Config.Presenter
{
    public class AddDialogFragmentSubPresenter : MvxSubpresenter
    { 
        public AddDialogFragmentSubPresenter(MvxAppPresenter appPresenter) : base(appPresenter)
        {
        }

        public override bool ShouldHandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request)
            => request.ViewModelType == typeof(AddFinanceOperationViewModel) || request.ViewModelType == typeof(AddContactViewModel) ||
            request.ViewModelType == typeof(AddBudgetViewModel);

        public override bool ShouldHandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute) 
            => viewModel.GetType() == typeof(AddFinanceOperationViewModel) || viewModel.GetType() == typeof(AddContactViewModel) ||
               viewModel.GetType() == typeof(AddBudgetViewModel);

        private int? statusBarColor;
        private int? navigationBarColor;
        
        public override Task<bool> HandleShowFragmentRequest(Type view, MvxFragmentPresentationAttribute attribute, MvxViewModelRequest request)
        {
            var mainActivity = (Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>()).Activity as MainActivity;
            mainActivity?.FloatingActionButton?.Hide();
            mainActivity.Window.AddFlags(WindowManagerFlags.TranslucentStatus | WindowManagerFlags.TranslucentNavigation);

            statusBarColor = statusBarColor ?? mainActivity.Window.StatusBarColor;
            navigationBarColor = navigationBarColor ?? mainActivity.Window.NavigationBarColor;
            mainActivity.Window.SetStatusBarColor(Color.Transparent);
            mainActivity.Window.SetNavigationBarColor(Color.Transparent);
            
            return base.HandleShowFragmentRequest(view, attribute, request);
        }

        public override Task<bool> HandleCloseFragmentRequest(IMvxViewModel viewModel, MvxFragmentPresentationAttribute attribute)
        {
            var mainActivity = (Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>()).Activity as MainActivity;

            mainActivity.Window.ClearFlags(WindowManagerFlags.TranslucentNavigation | WindowManagerFlags.TranslucentStatus);

            mainActivity.Window.SetStatusBarColor(new Color(statusBarColor.Value));
            mainActivity.Window.SetNavigationBarColor(new Color(navigationBarColor.Value));
            mainActivity?.FloatingActionButton?.PostDelayed(() =>
            {
                mainActivity?.FloatingActionButton.Show();
            }, 125);
            return base.HandleCloseFragmentRequest(viewModel, attribute);
        }
    }
}