using System;
using Airbnb.Lottie;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.Resources;
using Debts.ViewModel.AppGrowth;
using Foundation;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace Debts.iOS.ViewControllers.Main
{
    [MvxModalPresentation(ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen,
        ModalTransitionStyle = UIModalTransitionStyle.CoverVertical)]
    public class RateAppViewController : MvxViewController<RateAppViewModel>
    {
        private InkTouchController _inkTouchController;

        public RateAppViewController()
        {
        }

        public RateAppViewController(NSCoder coder) : base(coder)
        {
        }

        public RateAppViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        protected RateAppViewController(NSObjectFlag t) : base(t)
        {
        }

        protected internal RateAppViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;
            
            UIButton closeImageView = new UIButton(UIButtonType.Close); 
            closeImageView.TouchUpInside += ((e,a) =>
            {
                ViewModel.Skip.Execute();
            }); 
            
            
            LOTAnimationView firstAnimationView = LOTAnimationView.AnimationNamed("star_01");
            firstAnimationView.ContentMode = UIViewContentMode.ScaleAspectFit;
            firstAnimationView.LoopAnimation = false;
            firstAnimationView.Play();
            
            UILabel titleLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(18, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(128,128,128),
                Text = "Rate Us!",
                TextAlignment = UITextAlignment.Center
            };
            
            UILabel contentLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(162, 162, 162),
                Text = ViewModel.RateText,
                TextAlignment = UITextAlignment.Center,
                Lines = 0
            };
            
            LOTAnimationView secondAnimationView = LOTAnimationView.AnimationNamed("star_02");
            secondAnimationView.ContentMode = UIViewContentMode.ScaleAspectFit;
            secondAnimationView.LoopAnimation = false;
            secondAnimationView.Play();

            UIButton rateUsButton = new UIButton();
            rateUsButton.Layer.CornerRadius = 8;
            rateUsButton.Layer.MasksToBounds = true;
            rateUsButton.BackgroundColor = StartViewController.Colors.Primary;
            rateUsButton.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold);
            rateUsButton.ContentEdgeInsets = new UIEdgeInsets(6, 16, 6, 16); 
            rateUsButton.SetTitle("RATE NOW!", UIControlState.Normal);
            rateUsButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            
            _inkTouchController = new InkTouchController(rateUsButton);
            _inkTouchController.AddInkView();
             
            Add(closeImageView);
            Add(firstAnimationView);
            Add(titleLabel);
            Add(contentLabel);
            Add(secondAnimationView);
            Add(rateUsButton);
            
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            View.AddConstraints(
                    closeImageView.AtTopOfSafeArea(View, 12),
                    closeImageView.AtRightOf(View, 16),
                    
                    firstAnimationView.AtTopOfSafeArea(View, 48),
                    firstAnimationView.WithSameCenterX(View),
                    firstAnimationView.Height().EqualTo(160),
                    
                    titleLabel.Below(firstAnimationView, 12),
                    titleLabel.WithSameCenterX(View),
                    
                    contentLabel.AtLeftOf(View, 24),
                    contentLabel.AtRightOf(View, 24),
                    contentLabel.Below(titleLabel, 6),
                    
                    secondAnimationView.Below(contentLabel, 12),
                    secondAnimationView.WithSameCenterX(View),
                    secondAnimationView.Height().EqualTo(64), 
                    
                    rateUsButton.AtBottomOfSafeArea(View, 18),
                    rateUsButton.AtRightOf(View, 24)
            );

            var set = this.CreateBindingSet<RateAppViewController, RateAppViewModel>();

            set.Bind(rateUsButton)
                .To(x => x.Rate);
            
            set.Apply();
            
            View.BringSubviewToFront(closeImageView);
        }
    }
}