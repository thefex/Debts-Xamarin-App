using System.Threading.Tasks;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.Messenging.Messages.App;
using Debts.ViewModel;
using MvvmCross.Navigation;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace Debts.Services.AppGrowth
{
    public class DeepLinkHandler
    {
        private readonly IMvxNavigationService _navigationService;
        private readonly QueryCommandExecutor _queryCommandExecutor;

        public DeepLinkHandler(IMvxNavigationService navigationService, QueryCommandExecutor queryCommandExecutor)
        {
            _navigationService = navigationService;
            _queryCommandExecutor = queryCommandExecutor;
        }
        public async Task HandleDeepLink(FinanceOperation operation)
        {
            await _queryCommandExecutor.InvokeQuery(async sqLiteAsyncConnection =>
            {
                var operationRelatedContact = await sqLiteAsyncConnection.Table<ContactDetails>().FirstOrDefaultAsync(x => x.IsSharedContact &&
                                                                                                                           x.FirstName == operation.RelatedTo.FirstName &&
                                                                                                                           x.LastName == operation.RelatedTo.LastName);

                if (operationRelatedContact == null)
                {
                    await sqLiteAsyncConnection.InsertWithChildrenAsync(operation.RelatedTo);
                    ServicesLocation.Messenger.Publish(new ItemPublishedMessage<ContactDetails>(this, operation.RelatedTo));
                    operation.AssignedContactId = operation.RelatedTo.ContactPrimaryId; 
                }
                else
                {
                    operation.AssignedContactId = operationRelatedContact.ContactPrimaryId;
                    operation.RelatedTo = operationRelatedContact;
                }

                var operationInDatabase = await sqLiteAsyncConnection.Table<FinanceOperation>().FirstOrDefaultAsync(x => x.Title == operation.Title && x.CreatedAt == operation.CreatedAt);

                if (operationInDatabase == null)
                {
                    await sqLiteAsyncConnection.InsertWithChildrenAsync(operation);
                    ServicesLocation.Messenger.Publish(new ItemPublishedMessage<FinanceOperation>(this, operation));
                } 
            });

            await _navigationService.Navigate<FinanceDetailsViewModel, FinanceOperation>(operation);
        }
    }
}