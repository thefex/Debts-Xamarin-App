using System;
using Android.Views;
using Debts.Droid.Core.Extensions;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Debts.Droid.Config.Bindings
{
    public class VisibilityFadeInOutAndroidBinding : MvxAndroidTargetBinding
    {
        public VisibilityFadeInOutAndroidBinding(object target) : base(target)
        {
        }

        public override Type TargetType { get; } = typeof(bool);
        
        protected override void SetValueImpl(object target, object value)
        {
            if (value is bool booleanValue && target is View targetView)
            {
                if (booleanValue)
                    targetView.FadeIn();
                else
                    targetView.FadeOut(ViewStates.Gone);
            }
        }
    }
}