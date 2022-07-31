using System;
using Android.OS;
using Android.Runtime;
using Android.Support.Transitions;
using Android.Support.V7.Widget;
using Android.Views;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Expandable;
using Debts.Data;
using Debts.Droid.Activities;
using Debts.Model.Sections;
using Debts.ViewModel;
using Humanizer;
using MvvmCross.AdvancedRecyclerView;
using MvvmCross.AdvancedRecyclerView.Data;
using MvvmCross.AdvancedRecyclerView.Utils;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace Debts.Droid.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.FragmentRoot_SubPage, true, EnterAnimation = Resource.Animation.abc_slide_in_bottom, ExitAnimation = Resource.Animation.abc_slide_out_top, PopEnterAnimation = Resource.Animation.abc_fade_in, PopExitAnimation = Resource.Animation.abc_fade_out)]
    public class ContactDetailsFragment : BaseFragment<ContactDetailsViewModel, ContactDetails>
    {
        public ContactDetailsFragment() : base(Resource.Layout.contact_details)
        {
        }

        public ContactDetailsFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
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

            var recyclerView = inflatedView.FindViewById<MvxAdvancedExpandableRecyclerView>(Resource.Id.recyclerView);

            var accordionGroupController = recyclerView.ExpandableItemAdapter.GroupExpandController as AccordionMvxGroupExpandController;

            recyclerView.ExpandableItemAdapter.GroupItemBound += args =>
            {
                if (args.DataContext is MvxGroupedData groupedData && groupedData.Key is DetailsFinanceOperationsSection)
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


        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            
            inflater.Inflate(Resource.Menu.contact_details_toolbar, menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    ViewModel.Close.Execute();
                    return true; 
                case Resource.Id.action_contact_details_delete:
                    ViewModel.Delete.Execute();
                    return true;
                case Resource.Id.action_contact_details_call:
                    ViewModel.Call.Execute();
                    return true;
                case Resource.Id.action_contact_details_sms:
                    ViewModel.Sms.Execute();
                    return true;
            }
            
            return base.OnOptionsItemSelected(item); 
        }
    }
}