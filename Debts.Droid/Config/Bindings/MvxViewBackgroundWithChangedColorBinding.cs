using System;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Graphics.Drawable;
using Android.Support.V7.Content.Res;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Debts.Droid.Config.Bindings
{
    public class MvxViewBackgroundWithChangedColorBinding : MvxAndroidTargetBinding
    { 
        public MvxViewBackgroundWithChangedColorBinding(View view)
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

            var view = (ImageView)target;

            var valueStr = value as string;

            if (string.IsNullOrEmpty(valueStr))
                return;
 
            view.Post(() =>
            {
                Drawable unwrappedDrawable = AppCompatResources.GetDrawable(view.Context, Resource.Drawable.bubble_avatar); 
                Drawable wrappedDrawable = DrawableCompat.Wrap(unwrappedDrawable.Mutate());
                DrawableCompat.SetTint(wrappedDrawable, Color.ParseColor(valueStr));
                view.Background = wrappedDrawable;
            });
        }
    }
}