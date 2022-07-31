using System;
using Android.Runtime;
using Debts.Resources;
using Debts.ViewModel;
using Debts.ViewModel.FinancesViewModel;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.Finances
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.FragmentRoot, false, EnterAnimation = Resource.Animation.abc_fade_in, ExitAnimation = Resource.Animation.abc_fade_out, PopEnterAnimation = Resource.Animation.abc_fade_out, PopExitAnimation = Resource.Animation.abc_fade_in)]
    public class AllFinancesFragment : FinancesFragment<AllFinancesViewModel>
    {
        public AllFinancesFragment()
        {
        }

        public AllFinancesFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override string TitleText => TextResources.AllFinancesList_Title;
        protected override string EmptyListText => TextResources.AllFinancesList_EmptyListText;
    }
}