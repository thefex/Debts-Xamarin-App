using Debts.Services;

namespace Debts.iOS.Services
{
    public class iOSPlatform : IPlatform
    {
        public bool IsIos() => true;

        public bool IsAndroid() => false;
    }
}