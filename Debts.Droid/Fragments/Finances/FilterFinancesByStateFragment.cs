using System;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Debts.ViewModel.Finances;
using MvvmCross.Droid.Support.Design;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.Finances
{
    [MvxFragmentPresentation]
    public class FilterFinancesByStateFragment : MvxBottomSheetDialogFragment<FilterFinancesByStateViewModel>
    {
        public const string FilterFinancesByStateFragmentTag = "FilterFinancesByStateFragmentTag ";
        public FilterFinancesByStateFragment()
        {
            
        }

        public FilterFinancesByStateFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            return this.BindingInflate(Resource.Layout.select_state_filters, container, false);
        }
        
        public override int Theme => Resource.Style.Widget_AppTheme_BottomSheet;
    }
}