using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.App;
using Debts.ViewModel.Budget;

namespace Debts.Droid.Fragments.AddBudget
{
    public class MvxViewPagerAddBudgetFragmentAdapter : FragmentStatePagerAdapter
    {
        private readonly Context _context;
        private readonly AddBudgetViewModel _viewModel;

        public override int Count => 3;

        protected MvxViewPagerAddBudgetFragmentAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public MvxViewPagerAddBudgetFragmentAdapter(
            Context context, FragmentManager fragmentManager, AddBudgetViewModel viewModel)
            : base(fragmentManager)
        {
            _context = context;
            _viewModel = viewModel;
        }

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return new AddBudgetSelectTitleTypeFragment() {ViewModel = _viewModel};
                case 1:
                    return new AddBudgetSelectCategoryFragment() {ViewModel = _viewModel};
                case 2:
                    return new AddBudgetOperationSelectAmount() {ViewModel = _viewModel};
                default:
                    throw new InvalidOperationException("ViewPager fragment position unknown: " + position);
            }
        }
    }
}