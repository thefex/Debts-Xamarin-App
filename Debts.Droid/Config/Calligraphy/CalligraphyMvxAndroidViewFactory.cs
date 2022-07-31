using MvvmCross.Platforms.Android.Binding.Binders;

namespace Debts.Droid.Config.Calligraphy
{
    public class CalligraphyMvxAndroidViewFactory
        : IMvxAndroidViewBinderFactory
    {
        public virtual IMvxAndroidViewBinder Create(object source)
        {
            return new CalligraphyMvxAndroidViewBinder(source);
        }
    }
}