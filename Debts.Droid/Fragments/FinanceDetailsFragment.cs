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
    public class FinanceDetailsFragment : BaseFragment<FinanceDetailsViewModel, FinanceOperation>
    {
        private FinanceOperationDetailsFavAndFabController _financeOperationDetailsFavAndFabController;

        public FinanceDetailsFragment() : base(Resource.Layout.finance_details)
        {
        }

        public FinanceDetailsFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

     

        protected override void OnCreateViewInflated(View inflatedView)
        {
            base.OnCreateViewInflated(inflatedView);
             
            _financeOperationDetailsFavAndFabController = new FinanceOperationDetailsFavAndFabController(
                () => (Activity as MainActivity)?.SubViewTransitionInvoker.FloatingActionButton
                );

            (Activity as MainActivity).SetSupportActionBar(inflatedView.FindViewById<Toolbar>(Resource.Id.id_toolbar));
            (Activity as MainActivity).SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            (Activity as MainActivity).SupportActionBar.SetDisplayShowHomeEnabled(true);
            (Activity as MainActivity).SupportActionBar.SetDisplayShowTitleEnabled(false);
            
            HasOptionsMenu = true;

            var recyclerView = inflatedView.FindViewById<MvxAdvancedExpandableRecyclerView>(Resource.Id.recyclerView);
            var accordionGroupController = recyclerView.ExpandableItemAdapter.GroupExpandController as AccordionMvxGroupExpandController;

            recyclerView.ExpandableItemAdapter.GroupItemBound += args =>
            {
                if (args.DataContext is MvxGroupedData groupedData && groupedData.Key is DetailsNotesSection)
                {

                    args.ViewHolder.ItemView.Post(() =>
                    {
                        accordionGroupController.ItemHeight = args.ViewHolder.ItemView.Height;
                    });

                    var arrowView = args.ViewHolder.ItemView.FindViewById<View>(Resource.Id.arrow_view);

                    if (arrowView == null ||
                        (args.ViewHolder.ExpandStateFlags & ExpandableItemConstants.StateFlagIsUpdated) == 0)
                        return;

                    if ((args.ViewHolder.ExpandStateFlags & ExpandableItemConstants.StateFlagIsExpanded) != 0)
                    {
                        arrowView.Animate()
                            .Rotation(180)
                            .SetDuration(120)
                            .Start();
                    }
                    else
                    {
                        arrowView.Animate()
                            .Rotation(0)
                            .SetDuration(120)
                            .Start();
                    }
                }
            };
            var financeDetailsWalkthroughService = new FinanceDetailsWalkthroughService(Mvx.IoCProvider.Resolve<WalkthroughService>());
            inflatedView.PostDelayed(() =>
            {
                financeDetailsWalkthroughService.Initialize((Activity as MainActivity).FloatingActionButton, Activity);

                financeDetailsWalkthroughService.ShowIfPossible(Activity, ViewModel.ArePhoneRelatedFeaturesEnabled, !ViewModel.IsPaid);
            }, 325);

            var set = this.CreateBindingSet<FinanceDetailsFragment, FinanceDetailsViewModel>();
            
            set.Bind(_financeOperationDetailsFavAndFabController)
                .For(x => x.IsOperationPaid)
                .To(x => x.IsPaid);

            set.Bind(_financeOperationDetailsFavAndFabController)
                .For(x => x.IsFavorite)
                .To(x => x.IsFavourite);
            
            set.Apply();

        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            
            inflater.Inflate(Resource.Menu.finance_details_toolbar, menu);
            _financeOperationDetailsFavAndFabController.FavoriteMenuItem = menu.FindItem(Resource.Id.action_finance_details_fav);
            _financeOperationDetailsFavAndFabController.IsFavorite = ViewModel.IsFavourite; 
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    ViewModel.Close.Execute();
                    return true;
                case Resource.Id.action_finance_details_fav:
                    ViewModel.ToggleFavourite.Execute();
                    return true;
                case Resource.Id.action_finance_details_delete:
                    ViewModel.Delete.Execute();
                    return true;
                case Resource.Id.action_finance_details_add_note:
                    ViewModel.AddNote.Execute();
                    return true;
                case Resource.Id.action_finance_details_call:
                    ViewModel.Call.Execute();
                    return true;
                case Resource.Id.action_finance_details_sms:
                    ViewModel.Sms.Execute();
                    return true;
                case Resource.Id.action_finance_details_share:
                    ViewModel.Share.Execute();
                    return true;
            }
            
            
            return base.OnOptionsItemSelected(item); 
        }
    }
}