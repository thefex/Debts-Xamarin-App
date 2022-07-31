using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Support.V7.View;
using Android.Support.V7.Widget;
using Android.Views;
using Debts.Commands.Contacts;
using Debts.Droid.Fragments.Contacts;
using Debts.Droid.Messenging.Observers;
using Debts.Droid.Services.Walkthrough;
using Debts.Messenging;
using Debts.Model;
using Debts.Services.Settings;
using Debts.ViewModel.Budget;
using Debts.ViewModel.Contacts;
using MvvmCross;
using MvvmCross.AdvancedRecyclerView.Adapters.Expandable;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Activities
{
    [MvxActivityPresentation(ViewModelType = typeof(PickBudgetCategoryViewModel),
        ViewType = typeof(PickBudgetCategoryActivity))]
    [Activity(MainLauncher = false, Theme = "@style/BottomAppBarRelatedTheme",
        WindowSoftInputMode = SoftInput.AdjustPan, ScreenOrientation = ScreenOrientation.Portrait)]
    public class PickBudgetCategoryActivity : BasePickBudgetCategoryActivity<PickBudgetCategoryViewModel>
    {
        public PickBudgetCategoryActivity()
        {
        }

        public PickBudgetCategoryActivity(IntPtr ptr, JniHandleOwnership ownership) : base(ptr, ownership)
        {
        }
    }
}