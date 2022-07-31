namespace MvvmCross.Plugin.Location.Fused
{
    [MvxPlugin]
    [Preserve(AllMembers = true)]
    public class Plugin : IMvxPlugin
    {
        public void Load()
        {
            Mvx.IoCProvider.RegisterSingleton<IMvxLocationWatcher>(() => new MvxAndroidFusedLocationWatcher());
        }
    }
}