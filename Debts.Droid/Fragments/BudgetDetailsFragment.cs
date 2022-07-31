using System;
using Android.OS;
using Android.Runtime;
using Android.Support.Transitions;
using Android.Support.V7.Widget;
using Android.Views;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Expandable;
using Debts.Data;
using Debts.Droid.Activities;
using Debts.Droid.Core.ViewControllers;
using Debts.Droid.Services.Walkthrough;
using Debts.Model.Sections;
using Debts.Services.Settings;
using Debts.ViewModel;
using Debts.ViewModel.Budget;
using MvvmCross;
using MvvmCross.AdvancedRecyclerView;
using MvvmCross.AdvancedRecyclerView.Data;
using MvvmCross.AdvancedRecyclerView.Utils;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace Debts.Droid.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.FragmentRoot_SubPage, true,  EnterAnimation = Resource.Animation.abc_slide_in_bottom, ExitAnimation = Resource.Animation.abc_slide_out_top,PopEnterAnimation = Resource.Animation.abc_fade_in, PopExitAnimation = Resource.Animation.abc_fade_out)]
    public class BudgetDetailsFragment : BaseFragment<BudgetDetailsViewModel, BudgetItem>
    {
        public BudgetDetailsFragment() : base(Resource.Layout.budget_details)
        {
        }

        public BudgetDetailsFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
 
        protected override void OnCreateViewInflated(View inflatedView)
        {
            base.OnCreateViewInflated(inflatedView);
               
            (Activity as MainActivity).SetSupportActionBar(inflatedView.FindViewById<Toolbar>(Resource.Id.id_toolbar));
            (Activity as MainActivity).SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            (Activity as MainActivity).SupportActionBar.SetDisplayShowHomeEnabled(true);
            (Activity as MainActivity).SupportActionBar.SetDisplayShowTitleEnabled(false);
            
            HasOptionsMenu = true; 
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            
            inflater.Inflate(Resource.Menu.budget_details_toolbar, menu); 
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    ViewModel.Close.Execute();
                    return true; 
                case Resource.Id.action_finance_details_delete:
                    ViewModel.Delete.Execute();
                    return true;  
            }
             
            return base.OnOptionsItemSelected(item); 
        }
    }
}