using System;
using Android.OS;
using Android.Runtime;
using Android.Views;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.AddFinance
{
    [MvxFragmentPresentation]
    public class AddFinanceOperationSelectContactFragment : MvxFragment
    {
        public AddFinanceOperationSelectContactFragment()
        {
        }

        protected AddFinanceOperationSelectContactFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var inflatedView = this.BindingInflate(Resource.Layout.fragment_add_finance_operation_contact, container, false);
            return inflatedView;
        }
    }
}