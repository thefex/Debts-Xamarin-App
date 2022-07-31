using System.Threading.Tasks;

namespace Debts.ViewModel
{
    public interface IPhotoPickerModel
    {
        string PhotoPath { get; set; }

        TaskCompletionSource<bool> PhotoPicked { get; set; }
    }
}