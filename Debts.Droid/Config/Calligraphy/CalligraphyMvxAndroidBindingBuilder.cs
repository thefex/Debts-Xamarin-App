using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.Binders;

namespace Debts.Droid.Config.Calligraphy
{
    public class CalligraphyMvxAndroidBindingBuilder : MvxAndroidBindingBuilder
    {
        protected override IMvxAndroidViewBinderFactory CreateAndroidViewBinderFactory()
        {
            return new CalligraphyMvxAndroidViewFactory();
        }
    }
}