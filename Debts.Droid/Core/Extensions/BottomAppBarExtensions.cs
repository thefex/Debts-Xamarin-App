using System;
using System.Reflection;
using Android.Support.Design.BottomAppBar;

namespace Debts.Droid.Core.Extensions
{
    public static class BottomAppBarExtensions
    {
        public static void SlideUp(this BottomAppBar appBar)
        {
            var behavior = appBar.GetBehavior() as BottomAppBar.Behavior;
            var mi = behavior?.GetType().GetMethod("SlideUp", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] {typeof(BottomAppBar)}, null);
            if (mi != null) 
                mi.Invoke(behavior, new[] {appBar});
        }
    }
}