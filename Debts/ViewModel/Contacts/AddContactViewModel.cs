using System.Threading.Tasks;
using Debts.Commands;
using Debts.Commands.Contacts;
using Debts.DataAccessLayer;
using Debts.Services;
using Debts.Services.AppGrowth;
using MvvmCross.Commands;
using Plugin.Permissions.Abstractions;

namespace Debts.ViewModel.Contacts
{
    public class AddContactViewModel : BaseViewModel<string>, IPhotoPickerModel
    {
        private readonly PermissionService _permissionService;
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly AdvertisementService _advertisementService;

        public AddContactViewModel(PermissionService permissionService, 
            QueryCommandExecutor queryCommandExecutor,
            AdvertisementService advertisementService)
        {
            _permissionService = permissionService;
            _queryCommandExecutor = queryCommandExecutor;
            _advertisementService = advertisementService;
        }
        
        public string ContactName { get; set; }
        public string PhotoPath { get; set; }
        public TaskCompletionSource<bool> PhotoPicked{ get; set; }
        public string PhoneNumber { get; set; }

        public override void Prepare()
        {
            base.Prepare();
            
            if (_advertisementService.AreAdsAvailable)
                _advertisementService.PreloadFullScreenAd();
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            PhotoPicked?.SetResult(true);
        }

        public MvxCommand Close => new MvxExceptionGuardedCommand(() =>
        {
            ServicesLocation.NavigationService.Close(this);
            
            if (_advertisementService.AreAdsAvailable)
                _advertisementService.ShowFullScreenAd();
        });
        
        public MvxCommand PickPhoto => new PickPhotoCommandBuilder(this, _permissionService).BuildCommand();
 
        public MvxCommand AddContact => new AddContactCommandBuilder(this, _queryCommandExecutor).BuildCommand();
    }
}