using System;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.Graphics.Drawable;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Debts.Droid.Core.Extensions.Wrappers;
using Debts.Messenging.Messages;
using MvvmCross;
using MvvmCross.Platforms.Android;
using ArgbEvaluator = Android.Animation.ArgbEvaluator;

namespace Debts.Droid.Core.Extensions
{
    public static class ViewExtensions
    {
        public static View ForEachView<TOfType>(this View view, Action<TOfType> invokeAction) where TOfType : View
        {
            var castedView = view as TOfType;
            if (castedView != null)
                invokeAction(castedView);

            var viewAsViewGroup = view as ViewGroup;
            if (viewAsViewGroup == null)
                return view;

            for (var i = 0; i < viewAsViewGroup.ChildCount; ++i)
            {
                var childView = viewAsViewGroup.GetChildAt(i);
                ForEachView(childView, invokeAction);
            }

            return view;
        }

        public static View FadeIn(this View view, long duration = 350)
        {
            new Handler(Looper.MainLooper).Post(() =>
            {
                view.Animation?.Cancel();
                view.Animate()
                    .Alpha(1)
                    .SetDuration(duration)
                    .SetListener(
                        new AnimatorActionListenerWrapper(
                            x => view.Visibility = ViewStates.Visible,
                            x => { },
                            x => { },
                            x => { }))
                    .Start();
            });
            
            return view;
        }

        public static View FadeOut(this View view, ViewStates viewStateAfterAnimation, long duration = 350)
        {
            new Handler(Looper.MainLooper).Post(() =>
            {
                view.Animation?.Cancel();
                view.Animate()
                    .Alpha(0)
                    .SetDuration(duration)
                    .SetStartDelay(125)
                    .SetListener(
                        new AnimatorActionListenerWrapper(
                            x => { },
                            x => view.Visibility = viewStateAfterAnimation,
                            x => { },
                            x => { }))
                    .Start();
            });
            
            return view;
        }

        public static View AnimateBackgroundColor(this View view, Color newColor)
        {
            var currentColor = (view.Background as ColorDrawable)?.Color ?? Color.White;
            var colorValueAnimator =
                ValueAnimator.OfObject(new ArgbEvaluator(), currentColor.ToArgb(), newColor.ToArgb());
            colorValueAnimator.SetDuration(100);
            colorValueAnimator.Update += (e, a) =>
            {
                view.SetBackgroundColor(new Color((int) a.Animation.AnimatedValue));
            };
            colorValueAnimator.Start();
            return view;
        }

        public static void SdkSafeSetBackground(this View view, Drawable drawable)
        {
            var sdk = Build.VERSION.SdkInt;
            if (sdk < BuildVersionCodes.JellyBean)
            {
#pragma warning disable 618
                view.SetBackgroundDrawable(drawable);
#pragma warning restore 618
            }
            else
            {
                view.Background = drawable;
            }
        }
        
        public static View ShowWithTranslateAnimation(this View view)
        {
            view.Animate()
                .TranslationY(0)
                .SetInterpolator(new DecelerateInterpolator(2f))
                .Start();

            return view;
        }

        public static View HideWithTranslateAnimation(this View view, Action onHidden = null)
        {
            onHidden = onHidden ?? (() => { });
            var marginLayoutParams = view.LayoutParameters as ViewGroup.MarginLayoutParams;
            var margin = marginLayoutParams?.BottomMargin ?? 0;

            view.Animate()
                .TranslationY(view.Height + margin)
                .SetInterpolator(new AccelerateInterpolator(2f))
                .SetListener(new AnimatorActionListenerWrapper(x => {}, x => { onHidden(); }, x => { }, x => {}))
                .Start();

            return view;
        }

        public static View SetBackground(this View view, string drawableName)
        {
            if (string.IsNullOrWhiteSpace(drawableName))
                return view;

            var id = view.Context.Resources.GetIdentifier(drawableName, "drawable", view.Context.PackageName);

            if (drawableName.StartsWith("xvectorx_", StringComparison.OrdinalIgnoreCase))
            {
                view.SdkSafeSetBackground(VectorDrawableCompat.Create(view.Context.Resources, id, view.Context.Theme));
            }
            else
            {
                view.SetBackgroundResource(id);
            }
            return view;
        }
        
        public static Snackbar ConfigSnackbar(this Snackbar snack, View viewForMargin, Context context, ToastMvxMessage.ToastStyle style) {
            AddMargins(snack, viewForMargin);
            SetRoundBordersBg(context, snack, style);
            ViewCompat.SetElevation(snack.View, 6f);
            snack.View.SetFitsSystemWindows(true);
            
            if (style == ToastMvxMessage.ToastStyle.Error)
                snack.View.FindViewById<TextView>(Resource.Id.snackbar_text).SetTextColor(Color.White);
            
            return snack;
        }

        private static void AddMargins(Snackbar snack, View viewForMargin) {
            CoordinatorLayout.MarginLayoutParams layoutParams = (CoordinatorLayout.MarginLayoutParams) snack.View.LayoutParameters;
            int dpValue = 12;
            dpValue = (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, dpValue, snack.Context.Resources.DisplayMetrics);

            int bottomMargin =
                ((Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity).Window.NavigationBarColor == Color.Transparent) ? 
                    dpValue + (48/12 * dpValue) :
                    Math.Abs(viewForMargin.TranslationY) < double.Epsilon ? viewForMargin.Height + dpValue : dpValue;
            layoutParams.SetMargins(dpValue, 0, dpValue, bottomMargin); 
            snack.View.LayoutParameters = layoutParams; 
        }

        private static void SetRoundBordersBg(Context context, Snackbar snackbar, ToastMvxMessage.ToastStyle style)
        {
            int drawableId = style == ToastMvxMessage.ToastStyle.Info
                ? Resource.Drawable.material_v2_snackbar_info_background
                : Resource.Drawable.material_v2_snackbar_error_background;
	        
            snackbar.View.SdkSafeSetBackground(context.GetDrawable(drawableId));
        } 
    }
}