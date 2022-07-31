using System;
using Android.OS;
using Android.Runtime;
using Android.Views;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.AddBudget
{
    [MvxFragmentPresentation]
    public class AddBudgetSelectCategoryFragment : MvxFragment
    {
        public AddBudgetSelectCategoryFragment()
        {
        }

        protected AddBudgetSelectCategoryFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var inflatedView = this.BindingInflate(Resource.Layout.fragment_add_budget_category, container, false);
            return inflatedView;
        }
    }
}