using Debts.Services;

namespace Debts.Droid.Services
{
    public class DroidPlatform : IPlatform
    {
        public bool IsIos() => false;

        public bool IsAndroid() => true;
    }
}