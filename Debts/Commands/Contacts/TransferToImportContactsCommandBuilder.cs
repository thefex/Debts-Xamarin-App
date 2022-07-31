using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Debts.Data;
using Debts.Messenging.Messages;
using Debts.Messenging.Messages.App;
using Debts.Resources;
using Debts.Services;
using Debts.Services.Contacts;
using Debts.ViewModel.Contacts;
using DynamicData;
using MvvmCross;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace Debts.Commands.Contacts
{
    public class TransferToImportContactsCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly ContactListViewModel _contactListViewModel;
        private readonly PermissionService _permissionService;
        private readonly IPhoneContactsService _phoneContactsService;

        public TransferToImportContactsCommandBuilder(ContactListViewModel contactListViewModel, PermissionService permissionService, IPhoneContactsService phoneContactsService)
        {
            _contactListViewModel = contactListViewModel;
            _permissionService = permissionService;
            _phoneContactsService = phoneContactsService;
        }
        
        protected override async Task ExecuteCommandAction()
        {
            if (!await _permissionService.HasPermission(Permission.Contacts))
            {
                if (await _permissionService.IsPermissionDeniedInPast(Permission.Contacts))
                {
                    ServicesLocation.Messenger.Publish(new ToastMvxMessage(
                        this,
                        TextResources.Command_TransferToImportContact_ToastPermission_Content)
                    {
                        ActionCommand = new MvxExceptionGuardedCommand(() => { CrossPermissions.Current.OpenAppSettings(); }),
                        ActionText = TextResources.Command_TransferToImportContact_ToastPermission_ActionText,
                        Style = ToastMvxMessage.ToastStyle.Info
                    });
                    return;
                }
                
                var permissionRequestResponse = await _permissionService.RequestPermissions(Permission.Contacts);

                if (permissionRequestResponse != PermissionStatus.Granted)
                {
                    ServicesLocation.Messenger.Publish(new ToastMvxMessage(
                        this,
                        TextResources.Command_TransferToImportContact_ToastPermission_Content)
                    {
                        ActionCommand = new MvxExceptionGuardedCommand(() => { CrossPermissions.Current.OpenAppSettings(); }),
                        ActionText = TextResources.Command_TransferToImportContact_ToastPermission_ActionText,
                        Style = ToastMvxMessage.ToastStyle.Info
                    });
                    return;
                }
            }
            
            EnqueueAfterCommandExecuted(() =>
            {
                ServicesLocation.NavigationService.Navigate<ImportContactsViewModel, IEnumerable<Data.ContactDetails>>(Enumerable.Empty<Data.ContactDetails>());
            });
        }
         
    }
}