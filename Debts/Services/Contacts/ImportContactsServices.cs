using Plugin.Permissions.Abstractions;

namespace Debts.Services.Contacts
{
    public class ImportContactsServices
    {
        public IPhoneContactsService PhoneContactsService { get; }
        public PermissionService PermissionService { get; }

        public ImportContactsServices(IPhoneContactsService phoneContactsService, PermissionService permissionService)
        {
            PhoneContactsService = phoneContactsService;
            PermissionService = permissionService;
        }
    }
}