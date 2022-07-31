using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Extensions;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Resources;
using Debts.Services;
using Debts.Services.LocationService;
using Debts.ViewModel.Finances;
using MvvmCross.Base;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Debts.Commands.Finances
{
    public class AddNewFinanceOperationCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly AddFinanceOperationViewModel _addFinanceOperationViewModel;
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly LocationService _locationService;
        private readonly PermissionService _permissionService;

        public AddNewFinanceOperationCommandBuilder(
            AddFinanceOperationViewModel addFinanceOperationViewModel, 
            QueryCommandExecutor queryCommandExecutor, 
            LocationService locationService,
            PermissionService permissionService)
        {
            _addFinanceOperationViewModel = addFinanceOperationViewModel;
            _queryCommandExecutor = queryCommandExecutor;
            _locationService = locationService;
            _permissionService = permissionService;
        }
        
        protected override async Task ExecuteCommandAction()
        {
            bool hasLocationPermission = await _permissionService.HasPermission(Permission.Location);
            if (!hasLocationPermission)
            {
                if (await _permissionService.IsPermissionDeniedInPast(Permission.Location))
                {
                    TaskCompletionSource<bool> questionAwaiter = new TaskCompletionSource<bool>();

                    QuestionMessageDialogMvxMessage questionMessageDialogMvxMessage =
                        new QuestionMessageDialogMvxMessage(TextResources.Command_AddNewFinanceOperation_QuestionDialog_Title,
                            TextResources.Command_AddNewFinanceOperation_QuestionDialog_Content,
                            this)
                        {
                            OnNo = () => questionAwaiter.SetResult(false),
                            OnYes = () => questionAwaiter.SetResult(true),
                        };
                    ServicesLocation.Messenger.Publish(questionMessageDialogMvxMessage);

                    bool shouldGrantPermission = await questionAwaiter.Task;

                    if (shouldGrantPermission)
                    {
                        CrossPermissions.Current.OpenAppSettings();
                        return;
                    }
                }
                else
                {  
                    hasLocationPermission = (await _permissionService.RequestPermissions(Permission.Location) == PermissionStatus.Granted);
                } 
            }

            LocationData financeOperationLocation = null;
            if (hasLocationPermission)
            {
                _locationService.StartObserving();

                if (_locationService.MostRecentLocation == null)
                    await _locationService.GeolocationsObservable.FirstAsync();
                
                financeOperationLocation = new LocationData()
                {
                    Latitude = _locationService.MostRecentLocation.Coordinates.Latitude,
                    Longitude = _locationService.MostRecentLocation.Coordinates.Longitude
                };
            }

            var insertedFinanceOperation = await _queryCommandExecutor.Execute(new AddNewFinanceOperationCommandQuery(new FinanceOperation()
            {
                PaymentDetails = new PaymentDetails()
                {
                    DeadlineDate = _addFinanceOperationViewModel.Deadline.Value.GetEndOfTheDayDate(),
                    Amount = _addFinanceOperationViewModel.Amount.Value,
                    Currency = _addFinanceOperationViewModel.Currency
                },
                RelatedTo = _addFinanceOperationViewModel.PickedContact,
                Title = _addFinanceOperationViewModel.Title,
                Type = _addFinanceOperationViewModel.Type,
                Latitude = financeOperationLocation?.Latitude,
                Longitude = financeOperationLocation?.Longitude,
                CreatedAt = DateTime.UtcNow
            }));
            
            EnqueueAfterCommandExecuted(() =>
            {
                ServicesLocation.Messenger.Publish(new ItemPublishedMessage<FinanceOperation>(this, insertedFinanceOperation));
                _addFinanceOperationViewModel.Close.Execute();
            });
        }
    }
}