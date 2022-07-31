using System;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Debts.iOS.Config;
using Debts.iOS.ViewControllers.Base;
using Debts.ViewModel.Finances;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.ViewControllers.Menu
{
    public class FilterFinancesByStateRangeViewController : BaseViewController<FilterFinancesByStateViewModel, string>
    {
        private InkTouchController _inkTouchController;

        public FilterFinancesByStateRangeViewController()
        {
                
        }

        public FilterFinancesByStateRangeViewController(IntPtr ptr) : base(ptr)
        {
            
        }
  
        public override CGSize PreferredContentSize
        {
            get => new CGSize(View.Bounds.Width, 260+ UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom);
            set => base.PreferredContentSize = value;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.White;

            var hintColor = UIColor.FromRGB(168, 168, 168);
            var title = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(20, UIFontWeight.Regular),
                Text = "Filter by status",
                TextAlignment = UITextAlignment.Left,
                TextColor = AppColors.GrayForTextFieldContainer
            };

            UIView deadlineView = new UIView();

            UILabel deadlineLabel = new UILabel()
            {
                Text = "Show deadline exceed operations",
                TextColor =  hintColor,
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular)
            };
            UISwitch deadlineSwitch = new UISwitch()
            {
                OnTintColor = AppColors.Accent
            };
            
            deadlineView.Add(deadlineLabel);
            deadlineView.Add(deadlineSwitch);

            deadlineView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            deadlineView.AddConstraints(
                deadlineLabel.AtTopOf(deadlineView),
                deadlineLabel.AtLeftOf(deadlineView),
                deadlineLabel.ToLeftOf(deadlineSwitch, 12),
                deadlineLabel.AtBottomOf(deadlineView),
                    
                deadlineSwitch.AtRightOf(deadlineView, 12),
                deadlineSwitch.WithSameCenterY(deadlineView)
            );
  
            UIView activeView = new UIView();

            UILabel activeLabel = new UILabel()
            {
                Text = "Show active operations",
                TextColor =  hintColor,
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular)
            };
            UISwitch activeSwitch = new UISwitch()
            {
                OnTintColor = AppColors.Accent
            };
            
            activeView.Add(activeLabel);
            activeView.Add(activeSwitch);

            activeView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            activeView.AddConstraints(
                activeLabel.AtTopOf(activeView),
                activeLabel.AtLeftOf(activeView),
                activeLabel.ToLeftOf(activeSwitch, 12),
                activeLabel.AtBottomOf(activeView),
                    
                activeSwitch.AtRightOf(activeView, 12),
                activeSwitch.WithSameCenterY(activeView)
            );
            
            UIView paidOffView = new UIView();

            UILabel paidOffLabel = new UILabel()
            {
                Text = "Show paid-off operations",
                TextColor =  hintColor,
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular)
            };
            UISwitch paidOffSwitch = new UISwitch()
            {
                OnTintColor = AppColors.Accent
            };
            
            paidOffView.Add(paidOffLabel);
            paidOffView.Add(paidOffSwitch);

            paidOffView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            paidOffView.AddConstraints(
                paidOffLabel.AtTopOf(paidOffView),
                paidOffLabel.AtLeftOf(paidOffView),
                paidOffLabel.ToLeftOf(paidOffSwitch, 12),
                paidOffLabel.AtBottomOf(paidOffView),

                paidOffSwitch.AtRightOf(paidOffView, 12),
                paidOffSwitch.WithSameCenterY(paidOffView)
            );
            
            var filterButton = new UIButton();
            filterButton.Layer.CornerRadius = 18;
            filterButton.Layer.MasksToBounds = true;
            filterButton.BackgroundColor = AppColors.Primary;
            filterButton.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold);
            filterButton.ContentEdgeInsets = new UIEdgeInsets(12f, 36, 12f, 36); 
            filterButton.SetTitle("FILTER", UIControlState.Normal);
            filterButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            
            _inkTouchController = new InkTouchController(filterButton);
            _inkTouchController.AddInkView();
            
            Add(title);
            Add(deadlineView);
            Add(activeView);
            Add(paidOffView);
            Add(filterButton);
            
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            View.AddConstraints(
                title.AtTopOf(View, 3),
                title.AtLeftOf(View, 24),
                title.AtRightOf(View, 12),
                    
                deadlineView.Below(title, 0),
                deadlineView.WithSameLeft(title),
                deadlineView.WithSameRight(title),
                deadlineView.Height().EqualTo(36),
                    
                activeView.Below(deadlineView, 16),
                activeView.WithSameLeft(title),
                activeView.WithSameRight(title),
                activeView.Height().EqualTo(36),
                    
                paidOffView.Below(activeView, 16),
                paidOffView.WithSameLeft(title),
                paidOffView.WithSameRight(title),
                paidOffView.Height().EqualTo(36),
                    
                filterButton.Below(paidOffView, 12),
                filterButton.AtRightOf(View, 12),
                filterButton.AtBottomOfSafeArea(View, 12)
            );

            var set = this.CreateBindingSet<FilterFinancesByStateRangeViewController, FilterFinancesByStateViewModel>();

            set.Bind(activeSwitch)
                .To(x => x.IsActivePaymentEnabled);

            set.Bind(deadlineSwitch)
                .To(x => x.IsPaymentDeadlineExceedEnabled);

            set.Bind(paidOffSwitch)
                .To(x => x.IsPaidOffPaymentEnabled);
                
            set.Bind(filterButton)
                .To(x => x.Filter);
            
            set.Apply();
        }

        public static UIViewController Show(FilterFinancesByStateViewModel viewModel)
        {
            var mainViewController = UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController;
            var contentViewController = new FilterFinancesByStateRangeViewController()
            {
                ViewModel = viewModel
            };

            var bottomDrawerViewController = new BottomDrawerViewController();
            bottomDrawerViewController.SetTopCornersRadius(24, BottomDrawerState.Collapsed);
            bottomDrawerViewController.SetTopCornersRadius(24, BottomDrawerState.Expanded);
            bottomDrawerViewController.TopHandleHidden = false;
            bottomDrawerViewController.TopHandleColor = UIColor.LightGray;
            bottomDrawerViewController.ContentViewController = contentViewController; 
            
            mainViewController.PresentViewController(bottomDrawerViewController, animated: true, () =>
            { 
                //  ..  bottomDrawerViewController.SetContentOffsetY(-mainViewController.View.Bounds.Height*0.25f, true);
            });
            return bottomDrawerViewController;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ViewDidDisappear(animated);
        }
    }
}