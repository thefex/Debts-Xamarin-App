using System;
using System.Collections.Specialized;
using Android.Gms.Ads;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Adapter;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Expandable;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Utils;
using Debts.Data;
using Debts.Droid.Core.Converters;
using Debts.Droid.Core.Extensions;
using Debts.Droid.Core.Swipe;
using Debts.Droid.Services.Walkthrough;
using Debts.Model;
using Debts.Resources;
using Debts.Services.Settings;
using Debts.ViewModel;
using Debts.ViewModel.Contacts;
using MvvmCross;
using MvvmCross.AdvancedRecyclerView;
using MvvmCross.AdvancedRecyclerView.Adapters;
using MvvmCross.AdvancedRecyclerView.Adapters.Expandable;
using MvvmCross.AdvancedRecyclerView.Adapters.NonExpandable;
using MvvmCross.AdvancedRecyclerView.ViewHolders;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.FragmentRoot, false, EnterAnimation = Resource.Animation.abc_fade_in, ExitAnimation = Resource.Animation.abc_fade_out, PopEnterAnimation = Resource.Animation.abc_fade_in, PopExitAnimation = Resource.Animation.abc_fade_out)]
    public class ContactListFragment : BaseFragment<ContactListViewModel, string>
    {
        readonly SearchTextValueConverter _searchTextValueConverter = new SearchTextValueConverter();
        
        public ContactListFragment() : base(Resource.Layout.contact_list)
        {
            RetainInstance = true;
        }

        public ContactListFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override void OnCreateViewInflated(View inflatedView)
        {
            base.OnCreateViewInflated(inflatedView);
            
            recyclerView = inflatedView.FindViewById<MvxAdvancedExpandableRecyclerView>(Resource.Id.RecyclerView);
            
            recyclerView.MvxHeaderViewHolderBound += (args) =>
            {
                var titleTextView = args.Holder.ItemView.FindViewById<TextView>(Resource.Id.title);
                titleTextView.Text = TextResources.ContactList_Title;

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
                mvxExpandableItemAdapter.ChildSwipeItemPinnedStateController.ResetState();
                recyclerView.PostDelayed(() =>
                {
                    recyclerView.ExpandableItemAdapter.NotifyDataSetChanged();
                    ViewModel.SMS.Execute(item);
                }, 350);
            };
            
            mvxExpandableItemAdapter.ChildSwipeItemPinnedStateController.ForRightSwipe().Pinned += (item) =>
            {
                mvxExpandableItemAdapter.ChildSwipeItemPinnedStateController.ResetState();
                recyclerView.PostDelayed(() =>
                {
                    recyclerView.ExpandableItemAdapter.NotifyDataSetChanged();
                    ViewModel.Call.Execute(item);
                }, 350);
            }; 
             
            mvxExpandableItemAdapter.ChildItemSwipeBackgroundSet += (args) =>
            {
                int backgroundResourceId = -1;

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
            
            ContactListWalkthroughService contactListWalkthroughService = new ContactListWalkthroughService(
                Mvx.IoCProvider.Resolve<WalkthroughService>()
            );

            recyclerView.PostDelayed(() =>
            {
                contactListWalkthroughService.ShowIfPossible(Activity, Activity.FindViewById(Resource.Id.import_contacts_item));
            }, 375);

            recyclerView.ExpandableItemAdapter.ChildItemBound += (args) =>
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

            var set = this.CreateBindingSet<ContactListFragment, ContactListViewModel>();

            set.Bind(_searchTextValueConverter)
                .For(x => x.SearchQuery)
                .To(x => x.RecentSearchWord);

            set.Bind(this)
                .For(x => x.AreAdsAvailable)
                .To(x => x.AreAdsAvailable);
            
            set.Apply();
        }
        
        private MvxAdvancedExpandableRecyclerView recyclerView;
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

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.import_contacts_item)
            {
                ViewModel.ImportContacts.Execute();
                return true;
            }
            
            return base.OnOptionsItemSelected(item);
        }
    }
}