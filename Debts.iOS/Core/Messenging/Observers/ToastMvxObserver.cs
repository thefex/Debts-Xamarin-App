using System;
using Debts.iOS.Core.VIews;
using Debts.Messenging;
using Debts.Messenging.Messages;
using UIKit;

namespace Debts.iOS.Core.Messenging
{
    public class ToastMvxObserver : MvxMessageObserver<ToastMvxMessage>
    {
        private readonly Func<UIView> _rootViewProvider;

        public ToastMvxObserver(Func<UIView> rootViewProvider)
        {
            _rootViewProvider = rootViewProvider;
        }
    
        protected override void OnMessageArrived(ToastMvxMessage messageToHandle)
        {
            ToastViewBuilder toastViewBuilder = new ToastViewBuilder(_rootViewProvider);

            if (messageToHandle.ActionCommand != null && !string.IsNullOrEmpty(messageToHandle.ActionText))
            {
                toastViewBuilder.ShowWithAction(messageToHandle.Content, messageToHandle.Style, messageToHandle.ActionText, () => messageToHandle.DismissCommand?.Execute(null), () => messageToHandle.ActionCommand?.Execute(null));
            }
            else
            {
                toastViewBuilder.Show(messageToHandle.Content, messageToHandle.Style,
                    () => { messageToHandle.DismissCommand?.Execute(null); });
            }
        }
    }
}