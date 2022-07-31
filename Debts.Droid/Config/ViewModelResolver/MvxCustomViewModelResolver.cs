using MvvmCross;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android;
using MvvmCross.ViewModels;

namespace Debts.Droid.Config.ViewModelResolver
{
    internal abstract class MvxCustomViewModelResolver
    {
        public IMvxViewModel GetViewModel()
        {
            var currentActivity = Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity as MvxAppCompatActivity;
            return GetViewModelFromCurrentActivity(currentActivity);
        }

        protected abstract IMvxViewModel GetViewModelFromCurrentActivity(MvxAppCompatActivity currentActivity);
    }
}