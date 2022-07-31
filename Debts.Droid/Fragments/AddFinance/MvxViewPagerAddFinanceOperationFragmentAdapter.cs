using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.App;
using Debts.ViewModel;
using Debts.ViewModel.Finances;

namespace Debts.Droid.Fragments.AddFinance
{
    public class MvxViewPagerAddFinanceOperationFragmentAdapter : FragmentStatePagerAdapter
    {
        private readonly Context _context;
        private readonly AddFinanceOperationViewModel _viewModel;

        public override int Count => 4;

        protected MvxViewPagerAddFinanceOperationFragmentAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public MvxViewPagerAddFinanceOperationFragmentAdapter(
            Context context, FragmentManager fragmentManager, AddFinanceOperationViewModel viewModel)
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
                    return new AddFinanceOperationSelectTitleTypeFragment() {ViewModel = _viewModel};
                case 1:
                    return new AddFinanceOperationSelectContactFragment() {ViewModel = _viewModel};
                case 2:
                    return new AddFinanceOperationSelectAmount() {ViewModel = _viewModel};
                case 3:
                    return new AddFinanceOperationSelectDeadline() {ViewModel = _viewModel};
                default:
                    throw new InvalidOperationException("ViewPager fragment position unknown: " + position);
            }
        }
    }
}