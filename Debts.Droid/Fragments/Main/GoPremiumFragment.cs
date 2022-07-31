using System;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Debts.Data;
using Debts.Model;
using Debts.Resources;
using Debts.ViewModel;
using Debts.ViewModel.AppGrowth;
using MvvmCross.Droid.Support.Design;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.Contacts
{
    [MvxFragmentPresentation(ActivityHostViewModelType = typeof(MainViewModel), AddToBackStack = true, FragmentContentId = Resource.Id.pick_contacts_presenter, EnterAnimation = Resource.Animation.abc_slide_in_bottom, ExitAnimation = Resource.Animation.abc_slide_out_bottom)]
    public class GoPremiumFragment : MvxFragment<GoPremiumViewModel>
    {
        public GoPremiumFragment()
        {
        }

        public GoPremiumFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        private View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            view = this.BindingInflate(Resource.Layout.fragment_premium, container, false);
            view.FindViewById<TextView>(Resource.Id.text).Text = ViewModel.GoPremiumText.Replace("\r", "'\n");

            var orderButton = view.FindViewById<Button>(Resource.Id.premium_button);
            orderButton.Text = ViewModel.HasMonthlySubscription ? "PAY ONCE - LIFETIME PREMIUM!" : "GET YOUR PREMIUM!";
            orderButton.Click += (e, a) =>
            {
                if (ViewModel.HasMonthlySubscription)
                    ViewModel.BuyApp.Execute();
                else
                {
                    var menu = new OrderAppMenuFragment()
                    {
                        ViewModel = ViewModel,
                    }; 
                    menu.Show(ChildFragmentManager, menu.GetType().ToString());
                }
            };
            
            return view; 
        } 
    }
     
    public class OrderAppMenuFragment : MvxBottomSheetDialogFragment
    {
        public OrderAppMenuFragment()
        {
            
        }

        public OrderAppMenuFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

           
            var view = this.BindingInflate(Resource.Layout.fragment_premium_selector, container, false);
            view.ViewTreeObserver.GlobalLayout += (e, a) =>
            {
                BottomSheetDialog dialog = Dialog as BottomSheetDialog;
                var bottomSheet = dialog.FindViewById(Resource.Id.design_bottom_sheet);
                var behavior = BottomSheetBehavior.From(bottomSheet);
                behavior.State = BottomSheetBehavior.StateExpanded; 

            };
            var viewModel = ViewModel as GoPremiumViewModel;

            var navigationView = view.FindViewById<NavigationView>(Resource.Id.navigation_view);
            
             
            navigationView.NavigationItemSelected += (e, a) =>
            {
                
                switch (a.MenuItem.ItemId)
                {
                    case Resource.Id.nav_monthly:
                        viewModel.BuySubscription.Execute();
                        break;
                    case Resource.Id.nav_ownership:
                        viewModel.BuyApp.Execute();
                        break;           
                }
                Dismiss();
            };

            return view;
        }

        public override int Theme => Resource.Style.Widget_AppTheme_BottomSheet;
    }
}