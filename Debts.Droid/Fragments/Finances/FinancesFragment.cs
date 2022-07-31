using System;
using System.Threading.Tasks;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using Debts.Data;
using Debts.Droid.Activities;
using Debts.Droid.Core.Converters;
using Debts.Droid.Core.Extensions;
using Debts.Droid.Services.Walkthrough;
using Debts.Services.Settings;
using Debts.ViewModel.FinancesViewModel;
using MvvmCross;
using MvvmCross.AdvancedRecyclerView;
using MvvmCross.AdvancedRecyclerView.Adapters.Expandable;
using MvvmCross.AdvancedRecyclerView.ViewHolders;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;

namespace Debts.Droid.Fragments.Finances
{
    public abstract class FinancesFragment<TViewModel> : BaseFragment<TViewModel, string>, IFinanceListView
        where TViewModel : BaseFinancesViewModel
    {
        private MvxAdvancedRecyclerView recyclerView;
        private SearchTextValueConverter _titleSearchTextValueConverter = new SearchTextValueConverter();
        private SearchTextValueConverter _amountSearchTextValueConverter = new SearchTextValueConverter() { ColorHex = "#FFA500"};

        public FinancesFragment() : base(Resource.Layout.finances_list)
        {
            RetainInstance = true;
        }

        public FinancesFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        { 
            if (item.ItemId == Resource.Id.finance_list_nav_calendar)
            {
                ViewModel.FilterByDate.Execute();
            } else if (item.ItemId == Resource.Id.finance_list_nav_filter)
            {
                ViewModel.FilterByState.Execute();
            }
            
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnCreateViewInflated(View inflatedView)
        {
            base.OnCreateViewInflated(inflatedView);
            
          //  PostponeEnterTransition();
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
            
            //recyclerView.Post(() => { StartPostponedEnterTransition(); });
            var mvxExpandableItemAdapter = recyclerView.AdvancedRecyclerViewAdapter as MvxExpandableItemAdapter;
            
            mvxExpandableItemAdapter.ChildSwipeItemPinnedStateController.ForLeftSwipe().Pinned += (item) =>
            {
                ViewModel.Delete.Execute(item);
                mvxExpandableItemAdapter.ChildSwipeItemPinnedStateController.ForLeftSwipe().ResetState();
            };
            
            mvxExpandableItemAdapter.ChildSwipeItemPinnedStateController.ForRightSwipe().Pinned += (item) =>
            {
                ViewModel.Finalize.Execute(item);
                mvxExpandableItemAdapter.ChildSwipeItemPinnedStateController.ForRightSwipe().ResetState();
            };
            
            mvxExpandableItemAdapter.ChildItemSwipeBackgroundSet += (args) =>
            {
                switch (args.Type)
                {
                    case SwipeableItemConstants.DrawableSwipeNeutralBackground:
                        
                        break;
                    case SwipeableItemConstants.DrawableSwipeLeftBackground:
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.check_image_view).Visibility = ViewStates.Gone;
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.delete_image_view).Visibility = ViewStates.Visible;
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.root_view_layout).SetBackgroundResource(Resource.Drawable.bg_swipe_item_state_swiping_toLeft);
                        break;
                    case SwipeableItemConstants.DrawableSwipeRightBackground:
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.check_image_view).Visibility = ViewStates.Visible;
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.delete_image_view).Visibility = ViewStates.Gone;
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.root_view_layout).SetBackgroundResource(Resource.Drawable.bg_swipe_item_state_swiping_toRight);
                        break;
                }
            };

            mvxExpandableItemAdapter.ChildItemBound += (args) => {
                
                args.ViewHolder.ItemView.SetOnClickListener(new ClickListener(() =>
                {
                    args.ViewHolder.ItemView.FindViewById(Resource.Id.text_title).TransitionName = "text_title" + (args.DataContext as FinanceOperation).FinancePrimaryId;
                    args.ViewHolder.ItemView.FindViewById(Resource.Id.root_view).TransitionName = "root_view" + (args.DataContext as FinanceOperation).FinancePrimaryId;
                    args.ViewHolder.ItemView.FindViewById(Resource.Id.avatarView).TransitionName = "avatar" + (args.DataContext as FinanceOperation).FinancePrimaryId;

                    SelectedChildItemPosition = args.ViewHolder.AdapterPosition;
                    ViewModel.Details.Execute(args.DataContext); 
                }));


                var titleTextView = args.ViewHolder.ItemView.FindViewById<TextView>(Resource.Id.text_title);
                var priceTextView = args.ViewHolder.ItemView.FindViewById<TextView>(Resource.Id.amount_text_view);
                
                var set = args.ViewHolder.CreateBindingSet<MvxExpandableRecyclerViewHolder, FinanceOperation>();

                set.Bind(titleTextView)
                    .For(x => x.TextFormatted)
                    .To(x => x.Title)
                    .WithConversion(_titleSearchTextValueConverter);

                set.Bind(priceTextView)
                    .For(x => x.TextFormatted)
                    .To(x => x.PaymentDetails.FormattedAmount)
                    .WithConversion(_amountSearchTextValueConverter);
                
                set.Apply();
            };
 

            var bindingSet = this.CreateBindingSet<FinancesFragment<TViewModel>, TViewModel>();

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
            
            MainWalkthroughService walkthroughService = new MainWalkthroughService(Mvx.IoCProvider.Resolve<WalkthroughService>());
            inflatedView.PostDelayed(() =>
            {
                walkthroughService.Initialize((Activity as MainActivity).FloatingActionButton, Activity);
                walkthroughService.ShowIfPossible(Activity);
            }, 325);

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

        protected abstract string TitleText { get; }
        
        protected abstract string EmptyListText { get; }

        private int SelectedChildItemPosition { get; set; } = -1;
        public View GetSelectedTitleTextView()
        {
            if (SelectedChildItemPosition == -1)
                return null;

            var holder = recyclerView.FindViewHolderForAdapterPosition(SelectedChildItemPosition); 
            return holder?.ItemView?.FindViewById(Resource.Id.text_title);
        }

        public View GetAvatarImageView()
        {
            if (SelectedChildItemPosition == -1)
                return null;

            var holder = recyclerView.FindViewHolderForAdapterPosition(SelectedChildItemPosition); 
            return holder?.ItemView?.FindViewById(Resource.Id.avatarView);
        }

        public View GetRootView()
        {
            if (SelectedChildItemPosition == -1)
                return null;

            var holder = recyclerView.FindViewHolderForAdapterPosition(SelectedChildItemPosition); 
            return holder?.ItemView?.FindViewById(Resource.Id.root_view);
        }

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