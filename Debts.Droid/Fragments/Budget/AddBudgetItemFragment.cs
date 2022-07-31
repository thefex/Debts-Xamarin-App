using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Debts.Droid.Fragments.AddBudget;
using Debts.Droid.Fragments.AddFinance;
using Debts.Resources;
using Debts.ViewModel;
using Debts.ViewModel.Budget;
using Debts.ViewModel.Finances;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.Budget
{
    [MvxFragmentPresentation(ActivityHostViewModelType = typeof(MainViewModel), AddToBackStack = false, FragmentContentId = Resource.Id.add_fragment_presenter, EnterAnimation = Resource.Animation.abc_grow_fade_in_from_bottom, ExitAnimation = Resource.Animation.abc_shrink_fade_out_from_bottom)]
    public class AddBudgetItemFragment : MvxDialogFragment<AddBudgetViewModel>
    {
        public AddBudgetItemFragment()
        {
        }

        public AddBudgetItemFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = this.BindingInflate(Resource.Layout.fragment_add_budget, container, false);
            
            var pager = view.FindViewById<Android.Support.V4.View.ViewPager>(Resource.Id.pager);
            pager.Adapter = new MvxViewPagerAddBudgetFragmentAdapter(Activity, ChildFragmentManager, ViewModel);

            var tabLayout = view.FindViewById<TabLayout>(Resource.Id.tabLayout);
            tabLayout.SetupWithViewPager(pager);

            var finishButton = view.FindViewById<Button>(Resource.Id.finishButton);
            
            pager.PageSelected += (e, a) =>
            {
                ViewModel.CurrentSubPage = a.Position;
                finishButton.Text = a.Position == 2 ? TextResources.AddBudget_Title : TextResources.AddBudget_NEXT;
            };
            finishButton.Click += (e, a) =>
            {
                if (pager.CurrentItem == 2)
                    ViewModel.Add.Execute();
                else
                    ViewModel.Next.Execute();
            };

            var set = this.CreateBindingSet<AddBudgetItemFragment, AddBudgetViewModel>();

            set.Bind(pager)
                .For(x => x.CurrentItem)
                .To(x => x.CurrentSubPage);
            
            set.Apply();  
            
            return view;
        }
 
        //public override int Theme => Resource.Style.Widget_AppTheme_BottomSheet;
    }
}