using MvvmCross;
using MvvmCross.Logging;

namespace MvvmCross.Plugin.Location.Fused
{
    internal static class MvxPluginLog
    {
        internal static IMvxLog Instance { get; } = Mvx.Resolve<IMvxLogProvider>().GetLogFor("MvxPlugin");
    }
}