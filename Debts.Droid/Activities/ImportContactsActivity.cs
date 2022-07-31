using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Support.V7.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Debts.Commands.Contacts;
using Debts.Data;
using Debts.Droid.Core.Converters;
using Debts.Droid.Core.Extensions;
using Debts.Droid.Core.ViewControllers;
using Debts.Droid.Fragments.Contacts;
using Debts.Droid.Messenging.Observers;
using Debts.Droid.Services.Walkthrough;
using Debts.Messenging;
using Debts.Model;
using Debts.Resources;
using Debts.Services.Settings;
using Debts.ViewModel.Contacts;
using MvvmCross;
using MvvmCross.AdvancedRecyclerView;
using MvvmCross.AdvancedRecyclerView.Adapters.Expandable;
using MvvmCross.AdvancedRecyclerView.Adapters.NonExpandable;
using MvvmCross.AdvancedRecyclerView.ViewHolders;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Debts.Droid.Activities
{
    [MvxActivityPresentation(ViewModelType = typeof(ImportContactsViewModel), ViewType = typeof(ImportContactsActivity))]
    [Activity(MainLauncher = false, Theme = "@style/BottomAppBarRelatedTheme", WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = ScreenOrientation.Portrait)]
    public class ImportContactsActivity : BaseApplicationMvxActivity<ImportContactsViewModel, IEnumerable<ContactDetails>>
    {
        private View importProgressView;
        private View importButton;
        private BottomPanelController _bottomPanelController;
        readonly SearchTextValueConverter _searchTextValueConverter = new SearchTextValueConverter();
        private MvxAdvancedRecyclerView mainRecyclerView;
        private MvxRecyclerView bottomRecyclerView;

        public ImportContactsActivity()
        {
        }

        public ImportContactsActivity(IntPtr ptr, JniHandleOwnership ownership) : base(ptr, ownership)
        {
        }

        protected override void OnCreateView(Bundle bundle)
        {
            base.OnCreateView(bundle);
            SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.id_toolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.Title = TextResources.ViewModel_ContactListViewModel_Dialog_ImportContacts_Title;
            SupportActionBar.SetDisplayShowTitleEnabled(true);

            importProgressView = FindViewById(Resource.Id.ProgressBar);
            importButton = FindViewById(Resource.Id.ImportButton);

            mainRecyclerView = FindViewById<MvxAdvancedRecyclerView>(Resource.Id.RecyclerView);
            mainRecyclerView.MvxHeaderViewHolderBound += (args) =>
            {
                var adView = args.Holder.ItemView.FindViewById<AdView>(Resource.Id.ad_view);
                adView.LoadAdOrHideIfRequired(ViewModel.AreAdsAvailable);
            };
            
            _bottomPanelController = new BottomPanelController();
            _bottomPanelController.Initialize(FindViewById(Resource.Id.bottomPanel));

            bottomRecyclerView = FindViewById<MvxRecyclerView>(Resource.Id.bottom_recycler_view);
            bottomRecyclerView.SetLayoutManager(new LinearLayoutManager(this)
            {
                Orientation = OrientationHelper.Horizontal
            });
            
            var set = this.CreateBindingSet<ImportContactsActivity, ImportContactsViewModel>();

            set.Bind(_bottomPanelController)
                .For(x => x.IsPanelVisible)
                .To(x => x.HasAnyContactsToImport);
 
            set.Bind(_searchTextValueConverter)
                .For(x => x.SearchQuery)
                .To(x => x.RecentSearchWord);
            
            set.Apply();

            ImportContactsWalkthroughService importContactsWalkthroughService = new ImportContactsWalkthroughService(
                    Mvx.IoCProvider.Resolve<WalkthroughService>()
                );
            bottomRecyclerView.PostDelayed(() =>
            {
                importContactsWalkthroughService.ShowIfPossible(
                    this,
                    FindViewById(Resource.Id.action_import_search),
                    FindViewById(Resource.Id.overflowMenu)
                    );
            }, 375);
            
            (mainRecyclerView.AdvancedRecyclerViewAdapter as MvxExpandableItemAdapter).ChildItemBound  += (args) =>
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

            ViewModel.Load();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);
            
            MenuInflater.Inflate(Resource.Menu.import_contacts_toolbar, menu);
            var searchView = MenuItemCompat.GetActionView(menu.FindItem(Resource.Id.action_import_search)) as SearchView;

            searchView.QueryTextChange += (e, a) =>
            {
                ViewModel.SearchCommand.Execute(a.NewText);
            };
			 
            searchView.SetOnSearchClickListener(new ViewClickActionWrapper(() =>
            {
                SupportActionBar?.SetDisplayHomeAsUpEnabled(false);
            }));
			 
            searchView.Close += (e, a) =>
            {
                ViewModel.SearchCommand.Execute(string.Empty);
                searchView.OnActionViewCollapsed();
                SupportActionBar?.SetDisplayHomeAsUpEnabled(true);
            };
            
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Android.Resource.Id.Home)
            {
                Finish();
                return true;
            }

            if (item.ItemId == Resource.Id.action_import_reset)
            {
                ViewModel.ClearSelection.Execute();
                return true;
            }

            if (item.ItemId == Resource.Id.action_import_select_all)
            {
                ViewModel.SelectAll.Execute();
                return true;
            }
            
            return base.OnOptionsItemSelected(item);
        }

        protected override IEnumerable<IMvxMessageObserver> GetMessageObservers()
        {
            var observers = base.GetMessageObservers().ToList();
            
            observers.Add(new ReplaceViewOnLongOperationMessageObserver(() => importProgressView, () => importButton)
            {
                SenderTypesThatShouldBeProccesed = new [] { typeof(ImportContactsCommandBuilder) }
            });

            foreach(var blockViewObserver in BlockViewOnAsyncLongOperationMessageObserver.BuildObservers(FindViewById<ViewGroup>(Resource.Id.LayoutRoot)))
            {
                blockViewObserver.SenderTypesThatShouldBeProccesed = new[] {typeof(ImportContactsCommandBuilder)};
                observers.Add(blockViewObserver);
            }
            
            observers.Add(new DisableRecyclerViewItemTouchOnAsyncLongRunningOperationMessage(() => mainRecyclerView)
            {
                SenderTypesThatShouldBeProccesed = new[] {typeof(ImportContactsCommandBuilder)}
            });
            observers.Add(new DisableRecyclerViewItemTouchOnAsyncLongRunningOperationMessage(() => bottomRecyclerView)
            {
                SenderTypesThatShouldBeProccesed = new[] {typeof(ImportContactsCommandBuilder)}
            });
                
            return observers;
        }

        public override int LayoutId => Resource.Layout.activity_import_contacts;
 
        class ViewClickActionWrapper : Java.Lang.Object, View.IOnClickListener
        {
            private readonly Action _action;

            public ViewClickActionWrapper(Action action)
            {
                _action = action;
            }
            public void OnClick(View v)
            {
                _action();
            }
        }
    }
}