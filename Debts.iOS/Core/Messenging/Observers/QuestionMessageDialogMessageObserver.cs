using Debts.iOS.Config;
using Debts.Messenging;
using Debts.Messenging.Messages;
using FFImageLoading.Extensions;
using FFImageLoading.Work;
using MaterialComponents;
using MessageUI;
using UIKit;

namespace Debts.iOS.Core.Messenging
{
    internal class QuestionMessageDialogMessageObserver : MvxMessageObserver<QuestionMessageDialogMvxMessage>
    {
        private readonly UIViewController _activityReference;

        public QuestionMessageDialogMessageObserver(UIViewController activityReference)
        {
            _activityReference = activityReference;
        }

        protected override void OnMessageArrived(QuestionMessageDialogMvxMessage messageToHandle)
        {
            var alertController = new AlertController();
            alertController.Title = messageToHandle.Title;
            alertController.Message = messageToHandle.Content;
            alertController.CornerRadius = 6;
            alertController.TitleColor = AppColors.GrayForTextFieldContainer;
            alertController.ScrimColor = UIColor.Black.ColorWithAlpha(0.25f);
            alertController.ButtonTitleColor = AppColors.Accent;

            alertController.AddAction(AlertAction.Create(messageToHandle.OkButtonText.ToUpper(), ActionEmphasis.High, (args) =>
            {
                messageToHandle?.OnYes?.Invoke();
            }));
            alertController.AddAction(AlertAction.Create(messageToHandle.CancelButtonText.ToUpper(), ActionEmphasis.Medium, (args) =>
            {
                messageToHandle?.OnNo?.Invoke();
            }));
            UIViewController viewController = _activityReference;

            if (_activityReference.ModalViewController != null && _activityReference.ModalViewController is MFMessageComposeViewController == false)
            {
                viewController = _activityReference.ModalViewController;
            }
            
            viewController.PresentViewController(alertController, true, () =>
            {
                
            });
        }
    }
}