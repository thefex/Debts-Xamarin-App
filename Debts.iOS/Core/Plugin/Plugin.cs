using MvvmCross;
using MvvmCross.Converters;
using MvvmCross.Plugin;
using MvvmCross.Plugin.PictureChooser;

namespace Debts.iOS.Core.Plugin
{
    [MvxPlugin]
    [Foundation.Preserve(AllMembers = true)]
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.IoCProvider.RegisterType<IMvxPictureChooserTask, MvxImagePickerTask>();
            Mvx.IoCProvider.CallbackWhenRegistered<IMvxValueConverterRegistry>(RegisterValueConverter);
        }

        private void RegisterValueConverter()
        {
            Mvx.IoCProvider.Resolve<IMvxValueConverterRegistry>().AddOrOverwriteFrom(GetType().Assembly);
        }
    }
}