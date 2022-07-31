using System;
using System.Collections.Generic;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Debts.Droid.Core.Extensions;
using Debts.Droid.Fragments.Onboard;
using Debts.ViewModel;
using Debts.ViewModel.Finances;
using Java.Lang;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.AddFinance
{
    [MvxFragmentPresentation]
    public class AddFinanceOperationSelectTitleTypeFragment : MvxFragment
    {
        public AddFinanceOperationSelectTitleTypeFragment()
        {
        }

        protected AddFinanceOperationSelectTitleTypeFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var inflatedView = this.BindingInflate(Resource.Layout.fragment_add_finance_operation_titletype, container, false);
            
            var titleEditText = inflatedView.FindViewById<EditText>(Resource.Id.title_edit_text);
            titleEditText.EditorAction += (sender, args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    this.Activity.HideKeyboard();
                    var viewModel = ViewModel as AddFinanceOperationViewModel;
                    viewModel.Next.Execute();
                }
            };
            return inflatedView;
        }
    }
}