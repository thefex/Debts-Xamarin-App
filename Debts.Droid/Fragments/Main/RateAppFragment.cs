using System;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Airbnb.Lottie;
using Debts.ViewModel;
using Debts.ViewModel.AppGrowth;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Debts.Droid.Fragments.Contacts
{
    [MvxFragmentPresentation(ActivityHostViewModelType = typeof(MainViewModel), AddToBackStack = true, FragmentContentId = Resource.Id.pick_contacts_presenter, EnterAnimation = Resource.Animation.abc_slide_in_bottom, ExitAnimation = Resource.Animation.abc_slide_out_bottom)]
    public class RateAppFragment : MvxDialogFragment<RateAppViewModel>
    {
        public RateAppFragment()
        {
        }

        public RateAppFragment(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        private View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            view = this.BindingInflate(Resource.Layout.fragment_rate, container, false);
            view.FindViewById<TextView>(Resource.Id.text).Text = ViewModel.RateText.Replace("\r", "'\n");

            var firstAnimation = view.FindViewById<LottieAnimationView>(Resource.Id.first_animation);
            var secondAnimation = view.FindViewById<LottieAnimationView>(Resource.Id.second_animation);

            view.PostDelayed(() =>
            {
                firstAnimation.PlayAnimation();
                secondAnimation.PlayAnimation();
            }, 375);

            return view;
        } 
    }
}