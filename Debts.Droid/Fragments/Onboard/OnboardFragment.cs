using System;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Airbnb.Lottie;
using MvvmCross.ViewModels;

namespace Debts.Droid.Fragments.Onboard
{
    public class OnboardFragment : Fragment
    {
        public IMvxViewModel ViewModel { get; set; }

        private string _animationAssetName;
        private string _title;
        private string _text;

        public OnboardFragment(string animationAssetName, string title, string text)
        {
            _animationAssetName = animationAssetName;
            _title = title;
            _text = text;
        }

        public OnboardFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_onboard, container, false);
           
            var lottieAnimationView = view.FindViewById<LottieAnimationView>(Resource.Id.lottie_animation);
            lottieAnimationView.SetAnimation(_animationAssetName);
            view.FindViewById<TextView>(Resource.Id.title_text).Text = _title;
            view.FindViewById<TextView>(Resource.Id.text).Text = _text.Replace("\r", "'\n");

            var dip = 164;
            if (_animationAssetName.Contains("06"))
                dip = 186;
            
            var layoutParams = lottieAnimationView.LayoutParameters;
            layoutParams.Height = (int)TypedValue.ApplyDimension(
                ComplexUnitType.Dip,
                dip,
                lottieAnimationView.Resources.DisplayMetrics
            );
            ;
            lottieAnimationView.LayoutParameters = layoutParams;
            
            return view;
        }
    }
}