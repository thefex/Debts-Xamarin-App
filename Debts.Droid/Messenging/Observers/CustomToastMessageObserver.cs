using System;
using System.Linq;
using Debts.Droid.Activities;
using Debts.Droid.Core.ViewControllers;
using Debts.Droid.Fragments;
using Debts.Droid.Fragments.Budget;
using Debts.Messenging;
using Debts.Messenging.Messages;
using MvvmCross;
using MvvmCross.Platforms.Android;

namespace Debts.Droid.Messenging.Observers
{
    public class CustomToastMessageObserver : MvxMessageObserver<ToastMvxMessage>
    {
        private readonly Func<CustomSnackController> _customSnackController;
        
        public CustomToastMessageObserver(Func<CustomSnackController> customSnackController)
        {
            _customSnackController = customSnackController;
        }
        
        protected override void OnMessageArrived(ToastMvxMessage messageToHandle)
        {
            var primaryColor = "#570aeb";
            var errorColor = "#DC143C";

            var currentActivity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;
            bool needExtraMargin = false;

            if (currentActivity is MainActivity mainActivity)
            {
                var lastFragment = mainActivity.SupportFragmentManager.Fragments.LastOrDefault();
                if (lastFragment is AddFinanceOperationFragment || lastFragment is AddContactFragment || lastFragment is AddBudgetItemFragment)
                    needExtraMargin = true;
            }
            
            
            _customSnackController()?.Show(messageToHandle.Content, messageToHandle.Style, messageToHandle.DismissCommand, needExtraMargin,  messageToHandle.ActionText, messageToHandle.ActionCommand);
        }

        
    }
}