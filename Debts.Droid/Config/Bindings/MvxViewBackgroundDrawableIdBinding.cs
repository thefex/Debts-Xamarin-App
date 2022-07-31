using System;
using Android.Views;
using Debts.Droid.Core.Extensions;
using MvvmCross.Binding;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Debts.Droid.Config.Bindings
{
    public class MvxViewBackgroundDrawableIdBinding : MvxAndroidTargetBinding
    {
        public MvxViewBackgroundDrawableIdBinding(View view)
            : base(view)
        {
        }

        protected View ImageView => (View)Target;

        public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;

        public override Type TargetType => typeof(string);

        protected override void SetValueImpl(object target, object value)
        {
            if (value == null)
                return;

            var view = (View)target;

            var valueStr = value as string;
 
            view.Post(() =>
            {
                view.SetBackground(valueStr);
                //view.RequestLayout();
            });
        }
    }
}