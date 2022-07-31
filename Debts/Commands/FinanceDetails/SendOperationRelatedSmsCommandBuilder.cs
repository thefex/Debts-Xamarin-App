using System.Threading.Tasks;
using Debts.Data;
using Debts.Messenging.Messages;
using Debts.Resources;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.Settings;
using Xamarin.Essentials;

namespace Debts.Commands.FinanceDetails
{
    public class SendOperationRelatedSmsCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly AddNoteActionTriggerController _addNoteActionTriggerController;
        private readonly ShareLinkBuilderService _shareLinkBuilderService;
        private readonly FinanceOperation _forOperation;

        public SendOperationRelatedSmsCommandBuilder(AddNoteActionTriggerController addNoteActionTriggerController, ShareLinkBuilderService shareLinkBuilderService, FinanceOperation forOperation)
        {
            _addNoteActionTriggerController = addNoteActionTriggerController;
            _shareLinkBuilderService = shareLinkBuilderService;
            _forOperation = forOperation;
        }

        protected override bool ShouldNotifyAboutProgress => false;

        protected override async Task ExecuteCommandAction()
        {
            var shareMessageResponse = await _shareLinkBuilderService.BuildShareMessage(_forOperation);

            if (!shareMessageResponse.IsSuccess)
            {
                EnqueueAfterCommandExecuted(() =>
                {
                    ServicesLocation.Messenger.Publish(new MessageDialogMvxMessage(TextResources.Dialog_Error_Title, shareMessageResponse.FormattedErrorMessages, this)
                    {
                    });
                });
                return;
            }
             await Sms.ComposeAsync(new SmsMessage(shareMessageResponse.Results, _forOperation.RelatedTo.PhoneNumber));
            _addNoteActionTriggerController.AddNoteStarted(NoteType.Sms);
        }
    }
}