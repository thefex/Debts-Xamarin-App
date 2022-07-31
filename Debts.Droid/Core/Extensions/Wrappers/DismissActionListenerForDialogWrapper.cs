using System;
using Android.Content;
using Android.Runtime;

namespace Debts.Droid.Core.Extensions.Wrappers
{
    class DismissActionListenerForDialogWrapper : Java.Lang.Object, IDialogInterfaceOnDismissListener
    {
        private readonly Action _onDismiss;

        public DismissActionListenerForDialogWrapper(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public DismissActionListenerForDialogWrapper(Action onDismiss)
        {
            _onDismiss = onDismiss;
        }

        public void OnDismiss(IDialogInterface dialog)
        {
            _onDismiss();
        }
    }
}