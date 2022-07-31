using MvvmCross.ViewModels;
using Vision;

namespace Debts.iOS.Config.Presenter
{
    public interface ICustomPresenter
    {
        bool HandleShowRequest(MvxViewModelRequest request);

        bool HandleClose(IMvxViewModel viewModel);
        
        bool ShouldHandleRequest(MvxViewModelRequest request);

        bool ShouldHandleRequest(IMvxViewModel viewModel);
    }
}