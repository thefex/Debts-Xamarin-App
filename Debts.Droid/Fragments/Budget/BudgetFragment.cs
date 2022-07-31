using System;
using Android.Gms.Ads;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using Debts.Data;
using Debts.Droid.Activities;
using Debts.Droid.Core.Converters;
using Debts.Droid.Core.Extensions;
using Debts.Droid.Fragments.Finances;
using Debts.Droid.Services.Walkthrough;
using Debts.Resources;
using Debts.Services.Settings;
using Debts.ViewModel;
using Debts.ViewModel.Budget;
using Debts.ViewModel.FinancesViewModel;
using MvvmCross;
using MvvmCross.AdvancedRecyclerView;
using MvvmCross.AdvancedRecyclerView.Adapters.Expandable;
using MvvmCross.AdvancedRecyclerView.ViewHolders;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.Budget
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.FragmentRoot, false, EnterAnimation = Resource.Animation.abc_fade_in, ExitAnimation = Resource.Animation.abc_fade_out, PopEnterAnimation = Resource.Animation.abc_fade_out, PopExitAnimation = Resource.Animation.abc_fade_in)]
    public class BudgetFragment : BaseFragment<BudgetListViewModel, string>
    {
        private MvxAdvancedRecyclerView recyclerView;
        private SearchTextValueConverter _titleSearchTextValueConverter = new SearchTextValueConverter();
        private SearchTextValueConverter _amountSearchTextValueConverter = new SearchTextValueConverter() { ColorHex = "#FFA500"};

        public BudgetFragment() : base(Resource.Layout.budget_list)
        {
            RetainInstance = true;
        }

        public BudgetFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        { 
            if (item.ItemId == Resource.Id.budget_list_nav_calendar)
            {
                ViewModel.FilterByDate.Execute();
            } else if (item.ItemId == Resource.Id.budget_list_nav_filter)
            {
                ViewModel.FilterByStatus.Execute();
            }
            
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreateViewInflated(View inflatedView)
        {
            base.OnCreateViewInflated(inflatedView);
            
            var emptyTextView = inflatedView.FindViewById<TextView>(Resource.Id.empty_list_text);
            emptyTextView.Text = EmptyListText;
            
            recyclerView = inflatedView.FindViewById<MvxAdvancedRecyclerView>(Resource.Id.RecyclerView);
            recyclerView.MvxHeaderViewHolderBound += (args) =>
            {
                var titleTextView = args.Holder.ItemView.FindViewById<TextView>(Resource.Id.title);
                titleTextView.Text = TitleText;

                var searchView = args.Holder.ItemView.FindViewById<Android.Support.V7.Widget.SearchView>(Resource.Id.search_view);
                searchView.QueryTextChange -= SearchViewOnQueryTextChange;
                searchView.QueryTextChange += SearchViewOnQueryTextChange;  
			 
                searchView.Close -= SearchViewOnClose;
                searchView.Close += SearchViewOnClose;
                
                var adView = args.Holder.ItemView.FindViewById<AdView>(Resource.Id.ad_view);
                adView.LoadAdOrHideIfRequired(ViewModel.AreAdsAvailable);
            };
 
            var mvxExpandableItemAdapter = recyclerView.AdvancedRecyclerViewAdapter as MvxExpandableItemAdapter;
            
            mvxExpandableItemAdapter.ChildSwipeItemPinnedStateController.ForLeftSwipe().Pinned += (item) =>
            {
                ViewModel.Delete.Execute(item);
                mvxExpandableItemAdapter.ChildSwipeItemPinnedStateController.ForLeftSwipe().ResetState();
            };
            
            mvxExpandableItemAdapter.ChildItemSwipeBackgroundSet += (args) =>
            {
                switch (args.Type)
                {
                    case SwipeableItemConstants.DrawableSwipeNeutralBackground:
                        
                        break;
                    case SwipeableItemConstants.DrawableSwipeLeftBackground:
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.delete_image_view).Visibility = ViewStates.Visible;
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.root_view_layout).SetBackgroundResource(Resource.Drawable.bg_swipe_item_state_swiping_toLeft);
                        break;
                }
            };

            mvxExpandableItemAdapter.ChildItemBound += (args) => {
                var titleTextView = args.ViewHolder.ItemView.FindViewById<TextView>(Resource.Id.text_title);
                var priceTextView = args.ViewHolder.ItemView.FindViewById<TextView>(Resource.Id.amount_text_view);
                
                var set = args.ViewHolder.CreateBindingSet<MvxExpandableRecyclerViewHolder, BudgetItem>();

                set.Bind(titleTextView)
                    .For(x => x.TextFormatted)
                    .To(x => x.Title)
                    .WithConversion(_titleSearchTextValueConverter);

                set.Bind(priceTextView)
                    .For(x => x.TextFormatted)
                    .To(x => x.ShortFormattedAmount)
                    .WithConversion(_amountSearchTextValueConverter);
                
                set.Apply();
            };
  
            var bindingSet = this.CreateBindingSet<BudgetFragment, BudgetListViewModel>();

            bindingSet.Bind(_titleSearchTextValueConverter)
                .For(x => x.SearchQuery)
                .To(x => x.RecentSearchWord);

            bindingSet.Bind(_amountSearchTextValueConverter)
                .For(x => x.SearchQuery)
                .To(x => x.RecentSearchWord);
            
            bindingSet.Bind(this)
                .For(x => x.AreAdsAvailable)
                .To(x => x.AreAdsAvailable);

            bindingSet.Apply();
        }
        
        private bool _areAdsAvailable;
        public bool AreAdsAvailable 
        {
            get => _areAdsAvailable;
            set
            {
                if (_areAdsAvailable != value)
                {
                    _areAdsAvailable = value;
                    recyclerView.GetAdapter().NotifyDataSetChanged();
                }
            }
        }
        
        private void SearchViewOnClose(object sender, Android.Support.V7.Widget.SearchView.CloseEventArgs e)
        {
            ViewModel.SearchCommand.Execute(string.Empty);
            (sender as Android.Support.V7.Widget.SearchView).OnActionViewCollapsed();
        }

        private void SearchViewOnQueryTextChange(object sender, Android.Support.V7.Widget.SearchView.QueryTextChangeEventArgs e)
        {
            ViewModel.SearchCommand.Execute(e.NewText);
        }

        protected string TitleText => TextResources.BudgetList_Title;
        protected string EmptyListText => TextResources.BudgetList_EmptyListText;
        
        class ClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private readonly Action _action;

            public ClickListener(Action action)
            {
                _action = action;
            }

            public ClickListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
            {
            }

            public void OnClick(View v)
            {
                _action();
            }
        }
    }
}