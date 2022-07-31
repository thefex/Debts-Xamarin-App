using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Debts.Commands;
using Debts.Commands.ContactDetails;
using Debts.Commands.FinanceDetails;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.Messenging;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Model.NavigationData;
using Debts.Model.Sections;
using Debts.Resources;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.Phone;
using Debts.Services.Settings;
using Debts.ViewModel.AppGrowth;
using Debts.ViewModel.FinancesDetails;
using Humanizer;
using MvvmCross.Commands;
using Ninject.Activation;

namespace Debts.ViewModel
{
    public class FinanceDetailsViewModel : BaseViewModel<FinanceOperation>
    {
        private readonly PhoneCallServices _phoneCallServices;
        private readonly ShareLinkBuilderService _shareLinkBuilderService;
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly PremiumService _premiumService;
        private readonly AddNoteActionTriggerController _addNoteActionTriggerController = new AddNoteActionTriggerController();

        public FinanceDetailsViewModel(PhoneCallServices phoneCallServices, ShareLinkBuilderService shareLinkBuilderService, QueryCommandExecutor queryCommandExecutor,
            PremiumService premiumService)
        {
            _phoneCallServices = phoneCallServices;
            _shareLinkBuilderService = shareLinkBuilderService;
            _queryCommandExecutor = queryCommandExecutor;
            _premiumService = premiumService;

            MessageObserversController.AddObservers(
                new InvokeActionMessageObserver<ItemPublishedMessage<Note>>(msg =>
                {
                    (Sections.FirstOrDefault() as DetailsNotesSection)?.Notes?.Add(msg.Item);
                }))
                .AddObservers(
                    new InvokeActionMessageObserver<RequestFinalizeFinanceOperationMessage>(msg =>
                    {
                        FinalizeOperation.Execute();
                    })   
                )
                .AddObservers(new InvokeActionMessageObserver<SharedOrSmsNoteMessage>(msg => { HandleShareOrSmsNote(); }));
        }
        
        public override void Prepare(FinanceOperation parameter)
        {
            base.Prepare(parameter);
            Details = parameter;

            var notes = Details.Notes ?? Enumerable.Empty<Note>();
            Sections.Add(new DetailsNotesSection()
            {
                ImageName = "note_multiple_outline",
                Title = TextResources.ViewModel_FinancesDetailsViewModel_Notes,
                Notes = new ObservableCollection<Note>(notes) 
            });
            
            _phoneCallServices.Disconnected += PhoneCallServicesOnDisconnected;
        }

        private void PhoneCallServicesOnDisconnected()
        {
            _phoneCallServices.StopPhoneCall();
            string noteText = TextResources.ViewModel_FinancesDetailsViewModel_Note_CallDone;
                    
            new AddFinanceNoteCommandBuilder(_queryCommandExecutor, new AddFinanceDetailsNoteViewModel(_queryCommandExecutor)
            { 
                Operation = Details,
                Type = NoteType.Call,
                Note = noteText
            }).BuildCommand().Execute();
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            ServicesLocation.MessageQueue.ResendAndClearMessages<ItemPublishedMessage<Note>>();
            HandleShareOrSmsNote();
        }

        public async Task HandleShareOrSmsNote()
        {
            var requestedNoteType = _addNoteActionTriggerController.RequestedNoteType;
            _addNoteActionTriggerController.AddNoteFinished();

            if (requestedNoteType.HasValue)
            {
                string title = requestedNoteType == NoteType.Share ? "Share" : "SMS";
                string content = requestedNoteType == NoteType.Share ? TextResources.ViewModel_FinanceDetailsViewModel_Note_AskIfShared : TextResources.ViewModel_FinanceDetailsViewModel_Note_AskIfSms;

                TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

                QuestionMessageDialogMvxMessage askShouldAddNoteQuestion =
                    new QuestionMessageDialogMvxMessage(title, content, this)
                    {
                        OnNo = () => taskCompletionSource.SetResult(false),
                        OnYes = () => taskCompletionSource.SetResult(true),
                        CancelButtonText = TextResources.Button_No,
                        OkButtonText = TextResources.Button_Yes
                    };

                ServicesLocation.Messenger.Publish(askShouldAddNoteQuestion);

                bool shouldAddNote = await taskCompletionSource.Task;

                if (shouldAddNote)
                {
                    string noteText = requestedNoteType == NoteType.Share ? TextResources.ViewModel_FinanceDetailsViewModel_Note_YouHaveShared : TextResources.ViewModel_FinanceDetailsViewModel_Note_YouHaveSentSms;
                    
                    new AddFinanceNoteCommandBuilder(_queryCommandExecutor, new AddFinanceDetailsNoteViewModel(_queryCommandExecutor)
                    { 
                        Operation = Details,
                        Type = requestedNoteType.Value,
                        Note = noteText
                    }).BuildCommand().Execute();
                }
            }
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);
            _phoneCallServices.Disconnected -= PhoneCallServicesOnDisconnected;
        }

        public bool IsFavourite => Details.IsFavourite;

        public bool HasAnyNote => (Sections.FirstOrDefault(x => x is DetailsNotesSection) as DetailsNotesSection).Notes.Any();

        public bool IsPaid => Details.PaymentDetails.PaymentDate.HasValue;

        public bool ArePhoneRelatedFeaturesEnabled => !string.IsNullOrEmpty(Details.RelatedTo.PhoneNumber);
        
        public FinanceOperation Details { get; private set; }
        
        public IList<DetailsSection> Sections { get; } = new List<DetailsSection>(); 
        
        public MvxCommand Close => new MvxExceptionGuardedCommand(() => ServicesLocation.NavigationService.Close(this));
        
        public MvxCommand Call => new CallContactAsyncGuardedCommandBuilder(_phoneCallServices, Details.RelatedTo, _premiumService).BuildCommand();

        public MvxCommand Sms => new SendOperationRelatedSmsCommandBuilder(_addNoteActionTriggerController, _shareLinkBuilderService, Details).BuildCommand();
        
        public MvxCommand Map => new SeeFinanceOperationOnMapCommandBuilder(Details, _premiumService).BuildCommand();
        
        public MvxCommand Share => new ShareOperationCommandBuilder(_addNoteActionTriggerController, _shareLinkBuilderService, Details).BuildCommand();

        public MvxCommand ToggleFavourite => new ToggleFavouriteStateCommandBuilder(this, _queryCommandExecutor).BuildCommand();

        public MvxCommand Delete => new DeleteFinanceOperationCommandBuilder(_queryCommandExecutor, this).BuildCommand();
        
        public MvxCommand AddNote => new MvxExceptionGuardedCommand(() =>
            {
                if (!_premiumService.HasPremium)
                {
                    ServicesLocation.NavigationService.Navigate<GoPremiumViewModel>();
                    return;
                }
            
                ServicesLocation.NavigationService
                    .Navigate<AddFinanceDetailsNoteViewModel, AddFinanceOperationNoteNavigationData>(
                        new AddFinanceOperationNoteNavigationData(Details, null, NoteType.Default));
            });
        
        public MvxCommand FinalizeOperation => new FinalizeFinanceOperationCommandBuilder(this, _queryCommandExecutor).BuildCommand();
        
        public MvxCommand TransferToContact => new MvxExceptionGuardedCommand(() =>
            {
                ServicesLocation.NavigationService.Navigate<ContactDetailsViewModel, ContactDetails>(Details.RelatedTo);
            });
    }
}