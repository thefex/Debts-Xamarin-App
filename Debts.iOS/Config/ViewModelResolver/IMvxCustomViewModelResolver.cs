using MvvmCross;
using MvvmCross.ViewModels;

namespace Debts.iOS.Config.ViewModelResolver
{
    internal interface IMvxCustomViewModelResolver
    {
        IMvxViewModel GetViewModel();
    }
}