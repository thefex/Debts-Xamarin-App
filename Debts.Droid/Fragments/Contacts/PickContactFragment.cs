using System;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.BottomAppBar;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using Debts.Data;
using Debts.Droid.Core.Converters;
using Debts.Droid.Core.Extensions;
using Debts.Model;
using Debts.Resources;
using Debts.ViewModel;
using Debts.ViewModel.Contacts;
using MvvmCross.AdvancedRecyclerView;
using MvvmCross.AdvancedRecyclerView.Adapters.Expandable;
using MvvmCross.AdvancedRecyclerView.Adapters.NonExpandable;
using MvvmCross.AdvancedRecyclerView.ViewHolders;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.Contacts
{
    [MvxFragmentPresentation(ActivityHostViewModelType = typeof(MainViewModel), AddToBackStack = true, FragmentContentId = Resource.Id.pick_contacts_presenter, EnterAnimation = Resource.Animation.abc_slide_in_bottom, ExitAnimation = Resource.Animation.abc_slide_out_bottom)]
    public class PickContactFragment : MvxDialogFragment<PickContactViewModel>
    {
        readonly SearchTextValueConverter _searchTextValueConverter = new SearchTextValueConverter();
        
        public PickContactFragment()
        {
        }

        public PickContactFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        private View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            ViewModel.Load();

            view = this.BindingInflate(Resource.Layout.pick_contact_list, container, false);
             
            var bottomAppBar = view.FindViewById<BottomAppBar>(Resource.Id.pick_contacts_bottomAppBar);
            bottomAppBar.ReplaceMenu(Resource.Menu.contact_list_menu);
            bottomAppBar.MenuItemClick += (e, a) =>
            {
              if (a.Item.ItemId == Resource.Id.import_contacts_item)
                  ViewModel.ImportContacts.Execute();
            };

            var recyclerView = view.FindViewById<MvxAdvancedExpandableRecyclerView>(Resource.Id.RecyclerView);
            recyclerView.MvxHeaderViewHolderBound += (args) =>
            {
                var titleTextView = args.Holder.ItemView.FindViewById<TextView>(Resource.Id.title);
                titleTextView.Text = TextResources.PickContact_Title;
                
                var searchView = args.Holder.ItemView.FindViewById<Android.Support.V7.Widget.SearchView>(Resource.Id.search_view);
                searchView.QueryTextChange -= SearchViewOnQueryTextChange;
                searchView.QueryTextChange += SearchViewOnQueryTextChange;  
			 
                searchView.Close -= SearchViewOnClose;
                searchView.Close += SearchViewOnClose;

                
                var adView = args.Holder.ItemView.FindViewById<AdView>(Resource.Id.ad_view);
                adView.LoadAdOrHideIfRequired(ViewModel.AreAdsAvailable);
            };
            
            
            
            (recyclerView.AdvancedRecyclerViewAdapter as MvxExpandableItemAdapter).ChildItemBound  += (args) =>
            {
                var contactNameTextView = args.ViewHolder.ItemView.FindViewById<TextView>(Resource.Id.text_title);

                var bindingSet =
                    args.ViewHolder.CreateBindingSet<MvxAdvancedRecyclerViewHolder, SelectableItem<ContactDetails>>();

                bindingSet.Bind(contactNameTextView)
                    .For(x => x.TextFormatted)
                    .To(x => x.Item.FullName)
                    .WithConversion(_searchTextValueConverter);
                    
                bindingSet.Apply();
            };


            var mvxExpandableItemAdapter = recyclerView.AdvancedRecyclerViewAdapter as MvxExpandableItemAdapter;
            
            mvxExpandableItemAdapter.ChildSwipeItemPinnedStateController.ForLeftSwipe().Pinned += (item) =>
            {
                ViewModel.SMS.Execute(item);
                mvxExpandableItemAdapter.ChildSwipeItemPinnedStateController.ForLeftSwipe().ResetState();
            };
            
            mvxExpandableItemAdapter.ChildSwipeItemPinnedStateController.ForRightSwipe().Pinned += (item) =>
            {
                ViewModel.Call.Execute(item);
                mvxExpandableItemAdapter.ChildSwipeItemPinnedStateController.ForRightSwipe().ResetState();
            };
            
            mvxExpandableItemAdapter.ChildItemSwipeBackgroundSet += (args) =>
            {
                switch (args.Type)
                {
                    case SwipeableItemConstants.DrawableSwipeNeutralBackground:
                        
                        break;
                    case SwipeableItemConstants.DrawableSwipeLeftBackground:
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.sms_image_view).Visibility = ViewStates.Visible;
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.call_image_view).Visibility = ViewStates.Gone;
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.root_view_layout).SetBackgroundResource(Resource.Drawable.contact_bg_swipe_item_state_swiping_toLeft);
                        break;
                    case SwipeableItemConstants.DrawableSwipeRightBackground:
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.sms_image_view).Visibility = ViewStates.Gone;
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.call_image_view).Visibility = ViewStates.Visible;
                        args.ViewHolder.ItemView.FindViewById(Resource.Id.root_view_layout).SetBackgroundResource(Resource.Drawable.bg_swipe_item_state_swiping_toRight); 
                        break;
                }
            };

            var set = this.CreateBindingSet<PickContactFragment, PickContactViewModel>();

            set.Bind(_searchTextValueConverter)
                .For(x => x.SearchQuery)
                .To(x => x.RecentSearchWord);
            
            set.Apply();
            
            return view;
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
    }
}