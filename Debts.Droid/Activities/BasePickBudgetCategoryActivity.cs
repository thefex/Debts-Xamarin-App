using System;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Debts.Data;
using Debts.Droid.Core.Converters;
using Debts.Droid.Core.Extensions;
using Debts.Droid.Core.ViewControllers;
using Debts.Resources;
using Debts.ViewModel.Budget;
using MvvmCross.AdvancedRecyclerView;
using MvvmCross.AdvancedRecyclerView.Adapters.NonExpandable;
using MvvmCross.AdvancedRecyclerView.ViewHolders;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using SearchView = Android.Support.V7.Widget.SearchView;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Debts.Droid.Activities
{
    public abstract class BasePickBudgetCategoryActivity<TViewModel> : BaseApplicationMvxActivity<TViewModel, string>
        where TViewModel : PickBudgetCategoryViewModel
    {
        private View importProgressView;
        private View importButton;
        private BottomPanelController _bottomPanelController;
        readonly SearchTextValueConverter _searchTextValueConverter = new SearchTextValueConverter();
        private MvxAdvancedRecyclerView mainRecyclerView;
        private MvxRecyclerView bottomRecyclerView;

        public BasePickBudgetCategoryActivity()
        {
        }

        public BasePickBudgetCategoryActivity(IntPtr ptr, JniHandleOwnership ownership) : base(ptr, ownership)
        {
        }

        protected override void OnCreateView(Bundle bundle)
        {
            base.OnCreateView(bundle);
            SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.id_toolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.Title = TextResources.PickBudgetCategory_Title;
            SupportActionBar.SetDisplayShowTitleEnabled(true);

            importProgressView = FindViewById(Resource.Id.ProgressBar);
            importButton = FindViewById(Resource.Id.ImportButton);

            mainRecyclerView = FindViewById<MvxAdvancedRecyclerView>(Resource.Id.RecyclerView);
            mainRecyclerView.MvxHeaderViewHolderBound += (args) =>
            {
                var adView = args.Holder.ItemView.FindViewById<AdView>(Resource.Id.ad_view);
                adView.LoadAdOrHideIfRequired(ViewModel.AreAdsAvailable);
            }; 
            
            var set = this.CreateBindingSet<BasePickBudgetCategoryActivity<TViewModel>, TViewModel>();
  
            set.Bind(_searchTextValueConverter)
                .For(x => x.SearchQuery)
                .To(x => x.RecentSearchWord);
            
            set.Apply();
  
            (mainRecyclerView.AdvancedRecyclerViewAdapter as MvxNonExpandableAdapter).MvxViewHolderBound += (args) =>
            {
                var contactNameTextView = args.Holder.ItemView.FindViewById<TextView>(Resource.Id.text_title);

                var bindingSet =
                    (args.Holder as MvxAdvancedRecyclerViewHolder).CreateBindingSet<MvxAdvancedRecyclerViewHolder, BudgetCategory>();

                bindingSet.Bind(contactNameTextView)
                    .For(x => x.TextFormatted)
                    .To(x => x.Name)
                    .WithConversion(_searchTextValueConverter);
                    
                bindingSet.Apply();
            };

            ViewModel.Load();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);
            
            MenuInflater.Inflate(Resource.Menu.pick_budget_category_toolbar, menu);
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
            
            return base.OnOptionsItemSelected(item);
        } 
        
        public override int LayoutId => Resource.Layout.activity_pick_budget_category;
 
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