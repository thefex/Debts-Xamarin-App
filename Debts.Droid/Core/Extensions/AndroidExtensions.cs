using System;
using Android.OS;

namespace Debts.Droid.Core.Extensions
{
    public static class AndroidExtensions
    {
        public static void OnSdk(BuildVersionCodes apiLevel, Action beforeApi, Action onOrAfterApi)
        {
            if (Build.VERSION.SdkInt >= apiLevel)
                onOrAfterApi?.Invoke();
            else
                beforeApi();
        }
    }
}