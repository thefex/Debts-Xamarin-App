using System;
using System.Collections.Generic;
using System.Linq;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.ViewControllers.Base;
using Debts.iOS.ViewControllers.Onboard;
using Debts.ViewModel;
using Debts.ViewModel.OnboardViewModel;
using Foundation;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Extensions;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace Debts.iOS.ViewControllers
{
    [MvxRootPresentation]
    public class StartViewController : MvxPageViewController<StartViewModel>, IUIPageViewControllerDelegate
    {
        private UIPageControl pageControl;
        private InkTouchController _inkTouchController;

        public StartViewController() : base(UIPageViewControllerTransitionStyle.Scroll, UIPageViewControllerNavigationOrientation.Horizontal)
        {
        }

        public StartViewController(IntPtr handle) : base(handle)
        {
        }
 

        public override void ViewDidLoad()
        {
            AutomaticallyAdjustsScrollViewInsets = false;

            base.ViewDidLoad();

            var pageViewControllers = new List<UIViewController>()
            {
                new FirstOnboardViewController() { ViewModel = ViewModel.OnboardPagedViewModels[0] as FirstDebtsOnboardViewModel },
                new SecondPhoneContactsOnboardViewController() {ViewModel = ViewModel.OnboardPagedViewModels[1] as SecondPhoneContactsOnboardViewModel},
                new ThirdGpsOnboardViewController() { ViewModel = ViewModel.OnboardPagedViewModels[2] as ThirdGpsOnboardViewModel},
                new FourthNotifyDebtorViewController() { ViewModel = ViewModel.OnboardPagedViewModels[3] as FourthNotifyDebtorOnboardViewModel},
                new FifthBudgetOnboardViewController() { ViewModel = ViewModel.OnboardPagedViewModels[4] as BudgetOnboardViewModel},
                new SixOnboardViewController() { ViewModel = ViewModel.OnboardPagedViewModels[5] as SixMoreOnboardViewModel }
            };
            Delegate = this;
            View.BackgroundColor = UIColor.White;
            View.UserInteractionEnabled = true;

            foreach (var pageViewController in pageViewControllers)
                AddPage(pageViewController, new MvxPagePresentationAttribute());
            
            UIButton getStartedButton = new UIButton();
            getStartedButton.Layer.CornerRadius = 18;
            getStartedButton.Layer.MasksToBounds = true;
            getStartedButton.BackgroundColor = Colors.Primary;
            getStartedButton.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold);
            getStartedButton.ContentEdgeInsets = new UIEdgeInsets(12f, 12, 12f, 12); 
            getStartedButton.SetTitle("GET STARTED", UIControlState.Normal);
            getStartedButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            
            _inkTouchController = new InkTouchController(getStartedButton);
            _inkTouchController.AddInkView();
            
            pageControl = new UIPageControl()
            {
                Pages = 6,
                CurrentPage = 0,
                PageIndicatorTintColor = Colors.UnselectedPage,
                CurrentPageIndicatorTintColor = Colors.SelectedPage,
                UserInteractionEnabled = false
            };
             
            
            Add(pageControl);
            Add(getStartedButton);
            
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            View.AddConstraints( 
                
                    pageControl.AtBottomOf(View, 80),
                    pageControl.WithSameCenterX(View),
                    
                    getStartedButton.Below(pageControl, 12),
                    getStartedButton.WithSameCenterX(View)
                );

            var bindingSet = this.CreateBindingSet<StartViewController, StartViewModel>();

            bindingSet.Bind(getStartedButton)
                .To(x => x.SignIn);
            
            bindingSet.Apply();
        }

        private int currentPage = 0;
        [Export("pageViewController:willTransitionToViewControllers:")]
        public virtual void WillTransition(UIPageViewController pageViewController,
            UIViewController[] pendingViewControllers)
        {
            var targetVc = pendingViewControllers?.FirstOrDefault() as MvxViewController;

            if (targetVc?.ViewModel != null)
            {
                int index = ViewModel.GetIndexOfPage(targetVc.ViewModel);
                if (index != -1)
                {
                    currentPage = index;
                    pageControl.CurrentPage = index;
                }
            }
        }
        
           
        public class Colors
        {
            public static UIColor SelectedPage => UIColor.FromRGB(142, 205, 201);
            public static UIColor UnselectedPage => UIColor.FromRGB(241, 241, 241);
            
            public static UIColor Primary => UIColor.FromRGB(235, 9, 143);
        }
    }
}