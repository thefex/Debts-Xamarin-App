using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Debts.ViewModel.Budget;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Activities
{
    [MvxActivityPresentation(ViewModelType = typeof(PickBudgetCategoryForFilterViewModel),
        ViewType = typeof(PickBudgetCategoryForFilterActivity))]
    [Activity(MainLauncher = false, Theme = "@style/BottomAppBarRelatedTheme",
        WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = ScreenOrientation.Portrait)]
    public class
        PickBudgetCategoryForFilterActivity : BasePickBudgetCategoryActivity<PickBudgetCategoryForFilterViewModel>
    {
        public PickBudgetCategoryForFilterActivity()
        {
        }

        public PickBudgetCategoryForFilterActivity(IntPtr ptr, JniHandleOwnership ownership) : base(ptr, ownership)
        {
        }
    }
}