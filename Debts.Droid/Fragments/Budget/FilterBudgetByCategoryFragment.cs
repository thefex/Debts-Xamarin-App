using System;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Debts.ViewModel.Budget;
using Debts.ViewModel.Finances;
using MvvmCross.Droid.Support.Design;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.Budget
{
    [MvxFragmentPresentation]
    public class FilterBudgetByCategoryFragment : MvxBottomSheetDialogFragment<FilterBudgetByCategoryViewModel>
    {
        public const string FilterBudgetByCategoryTag = "FilterBudgetByCategoryFragment";
        public FilterBudgetByCategoryFragment()
        {
            
        }

        public FilterBudgetByCategoryFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            return this.BindingInflate(Resource.Layout.select_category_filter, container, false);
        }
        
        public override int Theme => Resource.Style.Widget_AppTheme_BottomSheet;
    }
}