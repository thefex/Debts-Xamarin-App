using System;
using Android.Runtime;
using Debts.Resources;
using Debts.ViewModel;
using Debts.ViewModel.Finances;
using Debts.ViewModel.FinancesViewModel;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.Finances
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.FragmentRoot, false, EnterAnimation = Resource.Animation.abc_fade_in, ExitAnimation = Resource.Animation.abc_fade_out)]
    public class MyLoansFragment : FinancesFragment<MyLoansViewModel>
    {
        public MyLoansFragment()
        {
        }

        public MyLoansFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override string TitleText => "Loans";
        protected override string EmptyListText => TextResources.MyLoans_EmptyListText;
    }
}