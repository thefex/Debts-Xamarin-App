using Debts.iOS.Config;
using Debts.Messenging;
using Debts.Messenging.Messages;
using MaterialComponents;
using UIKit;

namespace Debts.iOS.Core.Messenging
{
    internal class CheckboxQuestionMessageDialogMessageObserver : MvxMessageObserver<QuestionMessageWithCheckBoxMvxMessage>
    {
        private readonly UIViewController _activityReference;

        public CheckboxQuestionMessageDialogMessageObserver(UIViewController activityReference)
        {
            _activityReference = activityReference;
        }

        protected override void OnMessageArrived(QuestionMessageWithCheckBoxMvxMessage messageToHandle)
        {
            var alertController = new AlertController();
            alertController.Title = messageToHandle.Title;
            alertController.Message = messageToHandle.Content;
            alertController.CornerRadius = 6;
            alertController.TitleColor = AppColors.GrayForTextFieldContainer;
            alertController.ScrimColor = UIColor.Black.ColorWithAlpha(0.25f);
            alertController.ButtonTitleColor = AppColors.Accent;

            alertController.AddAction(AlertAction.Create("ok".ToUpperInvariant(), ActionEmphasis.High, (args) =>
            {
                messageToHandle?.OnYes?.Invoke(true);
            }));
            alertController.AddAction(AlertAction.Create( "no, thanks".ToUpper(), ActionEmphasis.Medium, (args) =>
            {
                messageToHandle?.OnNo?.Invoke(true);
            }));

            var viewController = _activityReference.ModalViewController ?? _activityReference;
            
            viewController.PresentViewController(alertController, true, () =>
            {
                
            });
        }
    }
}