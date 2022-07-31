using MvvmCross.ViewModels;

namespace Debts.ViewModel.OnboardViewModel
{
    public abstract class OnboardViewModel : BaseViewModel<string>
    {
        public string PagedViewId => GetType().FullName;
        
        public abstract string Title { get; }
        
        public abstract string Description { get; }
    }
}