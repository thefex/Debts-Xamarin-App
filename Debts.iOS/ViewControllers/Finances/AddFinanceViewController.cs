using System;
using System.Collections.Generic;
using System.Linq;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.iOS.Core.VIews;
using Debts.iOS.Utilities.Extensions;
using Debts.iOS.ViewControllers.Base;
using Debts.iOS.ViewControllers.Finances.AddFinance;
using Debts.iOS.ViewControllers.Onboard;
using Debts.Model.NavigationData;
using Debts.Resources;
using Debts.ViewModel;
using Debts.ViewModel.Finances;
using Debts.ViewModel.OnboardViewModel;
using Foundation;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Extensions;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace Debts.iOS.ViewControllers.Finances
{
    [MvxModalPresentation(ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen, ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve)]
    public class AddFinanceViewController : BaseViewController<AddFinanceOperationViewModel, AddFinanceOperationNavigationData>
    {
        private PageViewController pageViewController;
        private UIPageControl pageControl;
        private InkTouchController _inkTouchController;

        public AddFinanceViewController()
        {
        }

        public AddFinanceViewController(IntPtr handle) : base(handle)
        {
        }

        protected AddFinanceViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        private SlideViewKeyboardVisibilityManager _keyboardSlideVisibilityManager;
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            _keyboardSlideVisibilityManager = new SlideViewKeyboardVisibilityManager(() => View);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _keyboardSlideVisibilityManager?.Dispose();
            _keyboardSlideVisibilityManager = null;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.Black.ColorWithAlpha(0.65f);

            UIView viewContainer = new UIView()
            {
                BackgroundColor = UIColor.White,
                Layer = {CornerRadius = 4},
                ClipsToBounds = true
            };
            
            nextCompleteButton = new UIButton();
            nextCompleteButton.Layer.CornerRadius = 18;
            nextCompleteButton.Layer.MasksToBounds = true;
            nextCompleteButton.BackgroundColor = StartViewController.Colors.Primary;
            nextCompleteButton.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold);
            nextCompleteButton.ContentEdgeInsets = new UIEdgeInsets(12f, 36, 12f, 36); 
            nextCompleteButton.SetTitle(TextResources.AddFinanceOperation_NEXT, UIControlState.Normal);
            nextCompleteButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            
            _inkTouchController = new InkTouchController(nextCompleteButton);
            _inkTouchController.AddInkView();
              
            pageViewController = new PageViewController() { ViewModel = ViewModel };
            pageViewController.PageIndexChanged += (newIndex) => { ViewModel.CurrentSubPage = newIndex; };
            
            AddChildViewController(pageViewController);
            viewContainer.Add(pageViewController.View);
            
            pageControl = new UIPageControl()
            {
                Pages = 4,
                CurrentPage = 0,
                PageIndicatorTintColor = StartViewController.Colors.UnselectedPage,
                CurrentPageIndicatorTintColor = StartViewController.Colors.SelectedPage,
                UserInteractionEnabled = false
            };

            UIButton closeImageView = new UIButton(UIButtonType.Close); 
            closeImageView.TouchUpInside += ((e,a) =>
            {
                ViewModel.Close.Execute();
            });
            viewContainer.Add(closeImageView); 
            viewContainer.Add(pageControl);
            viewContainer.Add(nextCompleteButton);
            
            nextCompleteButton.TouchUpInside += (e, a) =>
            {
                if (ViewModel.CurrentSubPage == 3)
                    ViewModel.Add.Execute();
                else
                    ViewModel.Next.Execute();
            };

   
            Add(viewContainer);
            viewContainer.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            viewContainer.AddConstraints( 
                closeImageView.AtTopOf(viewContainer, 12),
                closeImageView.AtRightOf(viewContainer, 12),
                
                pageViewController.View.AtTopOf(viewContainer, 12),
                pageViewController.View.AtLeftOf(viewContainer),
                pageViewController.View.AtRightOf(viewContainer),
                pageViewController.View.AtBottomOf(viewContainer),
                    
                pageControl.AtBottomOf(viewContainer, 80),
                pageControl.WithSameCenterX(viewContainer),
                        
                nextCompleteButton.Below(pageControl, 12),
                nextCompleteButton.WithSameCenterX(viewContainer)
            );
            
            View.AddConstraints(
                    viewContainer.AtLeftOf(View, 12),
                    viewContainer.AtRightOf(View, 12),
                    viewContainer.Height().EqualTo(500),
                    viewContainer.WithSameCenterY(View)
                );
            
            var set = this.CreateBindingSet<AddFinanceViewController, AddFinanceOperationViewModel>();

            set.Bind(this)
                .For(x => x.PageIndex)
                .To(x => x.CurrentSubPage);
            
            set.Apply();
            
            this.AddCloseKeyboardOnTapHandlers(x => x is UIImageView ||
                                                    (x is UILabel label && label.Tag == EnterFinanceOperationNamePageViewController.OperationTypeLabelTag)
                                                );
            View.BringSubviewToFront(closeImageView);
        }

        private int _pageIndex;
        private UIButton nextCompleteButton;
        

        public int PageIndex
        {
            get => _pageIndex;
            set
            {
                if (_pageIndex != value)
                {
                    bool isForward = value > _pageIndex;
                    _pageIndex = value;
                    
                    nextCompleteButton.SetTitle(value == 3 ? TextResources.AddFinanceOperation_Title : TextResources.AddFinanceOperation_NEXT, UIControlState.Normal);
                    pageControl.CurrentPage = value;
                    pageViewController.SetViewControllers(new [] { pageViewController.Pages.ElementAt(value) }, isForward ? UIPageViewControllerNavigationDirection.Forward : UIPageViewControllerNavigationDirection.Reverse, true, 
                        (args) =>
                        {
                        });
                }
            }
        }

        class PageViewController : MvxPageViewController, IUIPageViewControllerDelegate
        {
            public PageViewController() : base(UIPageViewControllerTransitionStyle.Scroll, UIPageViewControllerNavigationOrientation.Horizontal)
            {
            }

            public PageViewController(IntPtr handle) : base(handle)
            {
            }

            public override void ViewDidLoad()
            {
                AutomaticallyAdjustsScrollViewInsets = false;

                base.ViewDidLoad();

                var pageViewControllers = new List<UIViewController>()
                {
                    new EnterFinanceOperationNamePageViewController() { ViewModel = ViewModel },
                    new AssignContactForAddFinancePageViewController() {ViewModel = ViewModel },
                    new EnterAmountCurrencyFinanceOperationPageViewController() { ViewModel = ViewModel },
                    new SelectDeadlineAddFinanceOperationPageViewController() { ViewModel = ViewModel }
                };
                Delegate = this;
                View.BackgroundColor = UIColor.White;
                View.UserInteractionEnabled = true;
 
                foreach (var pageViewController in pageViewControllers)
                    AddPage(pageViewController, new MvxPagePresentationAttribute());
             
            }

            [Export("pageViewController:willTransitionToViewControllers:")]
            public virtual void WillTransition(UIPageViewController pageViewController,
                UIViewController[] pendingViewControllers)
            {
                var targetVc = pendingViewControllers?.FirstOrDefault() as MvxViewController;

                if (targetVc != null)
                {
                    int index = 0;
                    if (targetVc is EnterFinanceOperationNamePageViewController)
                        index = 0;
                    else if (targetVc is AssignContactForAddFinancePageViewController)
                        index = 1;
                    else if (targetVc is EnterAmountCurrencyFinanceOperationPageViewController)
                        index = 2;
                    else if (targetVc is SelectDeadlineAddFinanceOperationPageViewController)
                        index = 3;
                    
                    OnPageIndexChanged(index);
                }
            }

            public event Action<int> PageIndexChanged;
            
               
            public class Colors
            {
                public static UIColor SelectedPage => UIColor.FromRGB(142, 205, 201);
                public static UIColor UnselectedPage => UIColor.FromRGB(241, 241, 241);
                
                public static UIColor Primary => UIColor.FromRGB(235, 9, 143);
            }

            protected virtual void OnPageIndexChanged(int obj)
            {
                PageIndexChanged?.Invoke(obj);
            }
        }

    }
}