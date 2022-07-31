using System;
using Android.Views;
using Debts.Droid.Core.Extensions;
using MvvmCross.Binding;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Debts.Droid.Config.Bindings
{
    public class MvxCustomItemLoadingBinding : MvxAndroidTargetBinding
    {
        private bool hasViewToFadeBeenVisibleBefore;
        
        public MvxCustomItemLoadingBinding(object obj) : base(obj)
        {
        }

        public override Type TargetType => typeof(bool);

        public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;

        protected override void SetValueImpl(object target, object value)
        {
            var view = target as ViewGroup;
            if (value == null || view == null)
                return;

            var isLoading = (bool)value;

            view.Post(() =>
            {
                var viewToFade = view.FindViewWithTag(view.Resources.GetString(Resource.String.notProgressViewTag));
                var progressBar = view.FindViewWithTag(view.Resources.GetString(Resource.String.progressViewTag));

                if (isLoading)
                {
                    hasViewToFadeBeenVisibleBefore = viewToFade?.Visibility == ViewStates.Visible;
                
                    if (hasViewToFadeBeenVisibleBefore)
                        viewToFade?.FadeOut(ViewStates.Invisible);
                    progressBar?.FadeIn();
                }
                else
                {
                    if (hasViewToFadeBeenVisibleBefore)
                        viewToFade?.FadeIn();
                
                    progressBar?.FadeOut(ViewStates.Gone);
                }    
            });
            
        }
    }
}