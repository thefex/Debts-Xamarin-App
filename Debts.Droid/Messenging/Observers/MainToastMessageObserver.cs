using System;
using Android.Graphics;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Debts.Droid.Core.Extensions;
using Debts.Messenging;
using Debts.Messenging.Messages;
using Object = Java.Lang.Object;

namespace Debts.Droid.Messenging.Observers
{
    internal class MainToastMessageObserver : MvxMessageObserver<ToastMvxMessage>
    {
        private readonly Func<View> _viewProvider;

        public MainToastMessageObserver(Func<View> viewProvider)
        {
            _viewProvider = viewProvider;
        }

        protected override void OnMessageArrived(ToastMvxMessage messageToHandle)
        {
            var view = _viewProvider();

            if (view == null)
                return;

            var snackBar = Snackbar.Make(view, messageToHandle.Content, (int) ToastLength.Short);
            if (!string.IsNullOrEmpty(messageToHandle.ActionText))
            {
                snackBar = 
                    snackBar
                        .SetAction(messageToHandle.ActionText, x => { messageToHandle.ActionCommand?.Execute(null); })
                        .SetActionTextColor(Color.White);
            }

            snackBar.AddCallback(new SnackbarCallback(() => messageToHandle?.DismissCommand?.Execute(null)));
            snackBar.ConfigSnackbar(view.FindViewById(Resource.Id.bottomAppBar), view.Context, messageToHandle.Style); 
            snackBar.Show();
        }

        class SnackbarCallback : Snackbar.BaseCallback
        {
            private readonly Action _dismissAction;

            public SnackbarCallback(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }

            public SnackbarCallback(Action dismissAction)
            {
                _dismissAction = dismissAction;
            }

            public override void OnDismissed(Object transientBottomBar, int e)
            {
                base.OnDismissed(transientBottomBar, e);

                if (e != Snackbar.Callback.DismissEventAction) _dismissAction?.Invoke();
            }
        } 
 
    }
}
    
