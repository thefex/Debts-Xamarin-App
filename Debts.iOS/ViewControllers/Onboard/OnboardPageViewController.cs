using System;
using Airbnb.Lottie;
using Cirrious.FluentLayouts.Touch;
using Debts.ViewModel.OnboardViewModel;
using Foundation;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using UIKit;

namespace Debts.iOS.ViewControllers.Onboard
{
   public abstract class OnboardPageViewController<TViewModel> : MvxViewController<TViewModel> where TViewModel : OnboardViewModel
    {
        public OnboardPageViewController()
        {
        }

        public OnboardPageViewController(IntPtr handle) : base(handle)
        {
        }

        protected OnboardPageViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            LOTAnimationView animationView = LOTAnimationView.AnimationNamed(AnimationName);
            animationView.ContentMode = UIViewContentMode.ScaleAspectFit;
            animationView.LoopAnimation = true;
            animationView.ContentScaleFactor = ScaleFactor;
            animationView.Play();
            
            UILabel titleLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(21, UIFontWeight.Semibold),
                Text = ViewModel.Title,
                TextAlignment = UITextAlignment.Center
            };

            UILabel descriptionLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular),
                Text = ViewModel.Description,
                Lines = 0,
                TextAlignment = UITextAlignment.Center
            };
            
            Add(animationView);
            Add(titleLabel);
            Add(descriptionLabel);

            
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            View.AddConstraints(
                    animationView.AtTopOfSafeArea(View, 24),
                    animationView.AtLeftOf(View),
                    animationView.AtRightOf(View),
                    animationView.Height().EqualTo(144),
                    
                    titleLabel.Below(animationView, 18),
                    titleLabel.WithSameCenterX(View),
                    
                    descriptionLabel.Below(titleLabel, 18),
                    descriptionLabel.AtLeftOf(View, 36),
                    descriptionLabel.AtRightOf(View, 36)
                );
        }

        protected virtual float ScaleFactor => 1;
        
        protected abstract string AnimationName { get; }
    }
}