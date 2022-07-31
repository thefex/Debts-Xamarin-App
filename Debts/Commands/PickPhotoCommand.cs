using System;
using System.IO;
using System.Threading.Tasks;
using Debts.Messenging.Messages;
using Debts.Model;
using Debts.Services;
using Debts.ViewModel;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Plugin.PictureChooser;

namespace Debts.Commands
{
    public class PickPhotoCommandBuilder : AsyncGuardedCommandBuilder
    {
        private readonly IPhotoPickerModel _photoPickerModel;
        private readonly PermissionService _permissionService;

        public PickPhotoCommandBuilder(IPhotoPickerModel photoPickerModel, PermissionService permissionService)
        {
            _photoPickerModel = photoPickerModel;
            _permissionService = permissionService;
        }
        protected override bool ShouldNotifyAboutProgress => false;

        protected override async Task ExecuteCommandAction()
        {
            _photoPickerModel.PhotoPicked = new TaskCompletionSource<bool>();
            TaskCompletionSource<LayerResponse<PictureDetails>> pickPictureTaskCompletionSource =
                new TaskCompletionSource<LayerResponse<PictureDetails>>();
            Mvx.IoCProvider.Resolve<IMvxPictureChooserTask>().ChoosePictureFromLibrary(1920, 100, (stream, name) =>
                {
                    pickPictureTaskCompletionSource.SetResult(new LayerResponse<PictureDetails>(new PictureDetails(stream, name)));
                },
                () => pickPictureTaskCompletionSource.SetResult(
                    new LayerResponse<PictureDetails>().SetAsFailureResponse()));

            var pickPictureResponse = await pickPictureTaskCompletionSource.Task;
            if (!pickPictureResponse.IsSuccess)
                return;

            if (ServicesLocation.IsAndroidPlatform)
                await _photoPickerModel.PhotoPicked.Task;
            
            string fileExtension = "png";
            string fileName = Guid.NewGuid().ToString("N") + "." + fileExtension;
            var pathToFile =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            using (var pictureFile = File.Create(pathToFile))
            {
                await pickPictureResponse.Results.Stream.CopyToAsync(pictureFile);
            }
 
            _photoPickerModel.PhotoPath = pathToFile;  
        }

        class PictureDetails
        {
            public Stream Stream { get; }
            public string PictureName { get; }

            public PictureDetails(Stream stream, string pictureName)
            {
                Stream = stream;
                PictureName = pictureName;
            }
        }
    }
}