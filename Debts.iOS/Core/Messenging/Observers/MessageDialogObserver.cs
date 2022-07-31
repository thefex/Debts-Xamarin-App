using Debts.iOS.Config;
using Debts.Messenging;
using Debts.Messenging.Messages;
using Debts.Resources;
using MaterialComponents;
using UIKit;

namespace Debts.iOS.Core.Messenging
{
    internal class MessageDialogMessageObserver : MvxMessageObserver<MessageDialogMvxMessage>
    {
        private readonly UIViewController _activityReference;

        public MessageDialogMessageObserver(UIViewController activityReference)
        {
            _activityReference = activityReference;
        }

        protected override void OnMessageArrived(MessageDialogMvxMessage messageToHandle)
        {
            var alertController = new AlertController();
            alertController.Title = messageToHandle.Title;
            alertController.Message = messageToHandle.Content;
            alertController.CornerRadius = 6;
            alertController.TitleColor = AppColors.GrayForTextFieldContainer;
            alertController.ScrimColor = UIColor.Black.ColorWithAlpha(0.25f);
            alertController.ButtonTitleColor = AppColors.Accent;
            
            alertController.AddAction(AlertAction.Create("OK", (args) => { }));
            if (_activityReference?.PresentedViewController is BottomDrawerViewController)
            {
                _activityReference.PresentedViewController.PresentViewController(alertController, true, () =>
                {
                    
                });
                return;
            }
            
            var viewController = _activityReference.ModalViewController ?? _activityReference;
            
            viewController.PresentViewController(alertController, true, () =>
            {
                
            });
        }
    }
}