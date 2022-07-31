using System;
using Airbnb.Lottie;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.iOS.ViewControllers.Base;
using Debts.iOS.ViewControllers.Menu;
using Debts.ViewModel.AppGrowth;
using Foundation;
using MaterialComponents;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace Debts.iOS.ViewControllers.Main
{
    [MvxModalPresentation(ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen, ModalTransitionStyle = UIModalTransitionStyle.CoverVertical)]
    public class GoPremiumViewController : MvxViewController<GoPremiumViewModel>
    {
        private InkTouchController _inkTouchController;

        public GoPremiumViewController()
        {
        }

        public GoPremiumViewController(NSCoder coder) : base(coder)
        {
        }

        public GoPremiumViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        protected GoPremiumViewController(NSObjectFlag t) : base(t)
        {
        }

        protected internal GoPremiumViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            View.BackgroundColor = UIColor.White;

            UIButton closeImageView = new UIButton(UIButtonType.Close);
            closeImageView.UserInteractionEnabled = true;
            closeImageView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ViewModel.Skip.Execute();
            }));

            LOTAnimationView firstAnimationView = LOTAnimationView.AnimationNamed("premium_bear");
            firstAnimationView.ContentMode = UIViewContentMode.ScaleAspectFit;
            firstAnimationView.LoopAnimation = true;
            firstAnimationView.Play();
            
            UILabel titleLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(18, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(128,128,128),
                Text = "Go Premium!",
                TextAlignment = UITextAlignment.Center
            };
            
            UILabel contentLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(162, 162, 162),
                Text = ViewModel.GoPremiumText,
                TextAlignment = UITextAlignment.Center,
                Lines = 0
            };

            UIButton goPremiumButton = new UIButton();
            goPremiumButton.Layer.CornerRadius = 8;
            goPremiumButton.Layer.MasksToBounds = true;
            goPremiumButton.BackgroundColor = StartViewController.Colors.Primary;
            goPremiumButton.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold);
            goPremiumButton.ContentEdgeInsets = new UIEdgeInsets(6, 16, 6, 16); 
            goPremiumButton.SetTitleColor(UIColor.White, UIControlState.Normal);
             
            _inkTouchController = new InkTouchController(goPremiumButton);
            _inkTouchController.AddInkView();
            
            Add(closeImageView);
            Add(firstAnimationView);
            Add(titleLabel);
            Add(contentLabel);
            Add(goPremiumButton);
            
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            View.AddConstraints(
                    closeImageView.AtTopOfSafeArea(View, 12),
                    closeImageView.AtRightOf(View, 16),
                    
                    firstAnimationView.AtTopOfSafeArea(View, 16),
                    firstAnimationView.WithSameCenterX(View),
                    firstAnimationView.Height().EqualTo(220),
                    
                    titleLabel.Below(firstAnimationView, -16),
                    titleLabel.WithSameCenterX(View),
                    
                    contentLabel.AtLeftOf(View, 24),
                    contentLabel.AtRightOf(View, 24),
                    contentLabel.Below(titleLabel, 6),
                  
                    goPremiumButton.Below(contentLabel, 16),
                    goPremiumButton.WithSameCenterX(View)
            );
            
            goPremiumButton.SetTitle(ViewModel.HasMonthlySubscription ? "PAY ONCE - LIFETIME PREMIUM!" : "GET YOUR PREMIUM!", UIControlState.Normal);
            goPremiumButton.TouchUpInside += (e, a) =>
            {
                if (ViewModel.HasMonthlySubscription)
                    ViewModel.BuyApp.Execute();
                else
                    PremiumMenuViewController.Show(ViewModel);
            };
            
            View.BringSubviewToFront(closeImageView);
        }
    }
}