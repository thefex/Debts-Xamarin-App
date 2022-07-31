using System;
using Android.Runtime;
using Debts.ViewModel.FinancesDetails;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.Stubs
{
    // subpresenter will handle presentation of this stub fragment
    [MvxFragmentPresentation]
    public class AddFinanceDetailsFragmentStub : MvxFragment<AddFinanceDetailsNoteViewModel>
    {
        public AddFinanceDetailsFragmentStub()
        {
        }

        public AddFinanceDetailsFragmentStub(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
    }
}