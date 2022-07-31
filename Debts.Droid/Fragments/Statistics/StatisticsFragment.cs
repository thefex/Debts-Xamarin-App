using System;
using Android.Runtime;
using Android.Views;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Expandable;
using Debts.Droid.Services.Walkthrough;
using Debts.Model.Sections;
using Debts.Services.Settings;
using Debts.ViewModel;
using Debts.ViewModel.Statistics;
using MvvmCross;
using MvvmCross.AdvancedRecyclerView;
using MvvmCross.AdvancedRecyclerView.Data;
using MvvmCross.AdvancedRecyclerView.Utils;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using SpotlightXamarin;

namespace Debts.Droid.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.FragmentRoot, false, EnterAnimation = Resource.Animation.abc_fade_in, ExitAnimation = Resource.Animation.abc_fade_out, PopEnterAnimation = Resource.Animation.abc_fade_in, PopExitAnimation = Resource.Animation.abc_fade_out)]
    public class StatisticsFragment : BaseFragment<StatisticsViewModel, string>
    {
        public StatisticsFragment() : base(Resource.Layout.statistics)
        {
            RetainInstance = true;
        }

        public StatisticsFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override void OnCreateViewInflated(View inflatedView)
        {
            base.OnCreateViewInflated(inflatedView);
            
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
            
            StatisticsWalkthroughService statisticsWalkthroughService = new StatisticsWalkthroughService(Mvx.IoCProvider.Resolve<WalkthroughService>());
            inflatedView.PostDelayed(() =>
            {
                var menuItem = Activity.FindViewById(Resource.Id.statistics_list_nav_calendar);
                statisticsWalkthroughService.ShowIfPossible(Activity, menuItem);
            }, 375);
        }
        
        public override bool OnOptionsItemSelected(IMenuItem item)
        { 
            if (item.ItemId == Resource.Id.statistics_list_nav_calendar)
            {
                ViewModel.FilterByDate.Execute();
                return true;
            }  
            
            return base.OnOptionsItemSelected(item);
        } 
    }
}