using System;
using System.Collections.Generic;
using System.Linq;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Debts.Converters;
using Debts.Data;
using Debts.iOS.Config;
using Debts.iOS.Core.VIews;
using Debts.iOS.Utilities.Extensions;
using Debts.iOS.ViewControllers.Base;
using Debts.Messenging;
using Debts.ViewModel.Budget;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Plugin.Color.Platforms.Ios;
using UIKit;

namespace Debts.iOS.ViewControllers.Menu
{
    public class FilterBudgetByCategoryViewController : BaseViewController<FilterBudgetByCategoryViewModel, string>
    {
        private InkTouchController _inkTouchController;
        public FilterBudgetByCategoryViewController()
        {
                
        }

        public FilterBudgetByCategoryViewController(IntPtr ptr) : base(ptr)
        {
            
        }

        protected override IEnumerable<IMvxMessageObserver> GetMessageObservers()
        {
            var observers = base.GetMessageObservers().ToList();
            
             

            return observers;
        }

        public override CGSize PreferredContentSize
        {
            get => new CGSize(View.Bounds.Width, 215+ UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom);
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
                Text = "Filter by category",
                TextAlignment = UITextAlignment.Left,
                TextColor = AppColors.GrayForTextFieldContainer
            };

            UIView pickCategoryView = PrepareContainerView();
 
            
            
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
            Add(pickCategoryView);
            Add(filterButton);
            
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            View.AddConstraints(
                title.AtTopOf(View, 3),
                title.AtLeftOf(View, 24),
                title.AtRightOf(View, 12),
                    
                pickCategoryView.Below(title, 0),
                pickCategoryView.WithSameLeft(title),
                pickCategoryView.WithSameRight(title),
                    
                filterButton.Below(pickCategoryView, 12),
                filterButton.AtRightOf(View, 12),
                filterButton.AtBottomOfSafeArea(View, 12)
            );

            var set = this.CreateBindingSet<FilterBudgetByCategoryViewController, FilterBudgetByCategoryViewModel>();
         
            set.Bind(filterButton)
                .To(x => x.Filter);
            
            set.Bind(this)
                .For(x => x.SelectedCategory)
                .To(x => x.SelectedFilterCategory);

            set.Bind(categoryName)
                .To(x => x.SelectedFilterCategory)
                .WithConversion(new PickedCategoryToTextValueConverter());
            
            set.Apply();
        }

        private BudgetCategory _budgetCategory;
        public BudgetCategory SelectedCategory
        {
            get => _budgetCategory;
            set
            {
                _budgetCategory = value;
                
                if (value == null)
                    new AvatarGenerator().GenerateAvatar(avatarImageView, value?.ToString() ?? "?", string.Empty, 15, 64, UIColor.White, UIColor.Gray);
                else
                {
                    avatarImageView.Layer.CornerRadius = 32;
                    avatarImageView.ContentMode = UIViewContentMode.Center;
                    avatarImageView.Image = UIImage.FromBundle(value.AssetName).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                    avatarImageView.TintColor = UIColor.White;
                    avatarImageView.BackgroundColor = MvvmCross.Plugin.Color.MvxHexParser.ColorFromHexString(value.ColorHex).ToNativeColor();
                }
            }
        }
        
        private UIImageView avatarImageView;
        private UILabel categoryName;
        
        private UIView PrepareContainerView()
        { 
            var containerView = new UIView {BackgroundColor = AppColors.GrayForFieldContainer};
            containerView.Layer.CornerRadius = 6;
            containerView.UserInteractionEnabled = true;
            containerView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ViewModel.SelectCategory.Execute();
            }));

            avatarImageView = new UIImageView() {ContentMode = UIViewContentMode.ScaleAspectFit};
            categoryName = new UILabel()
            {
                TextColor = AppColors.GrayForTextFieldContainer,
                Font = UIFont.SystemFontOfSize(17, UIFontWeight.Regular)
            };
             
            containerView.AddSubviews(avatarImageView, categoryName);
            containerView.AddShadow();
            containerView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            containerView.AddConstraints(
                avatarImageView.Height().EqualTo(64),
                avatarImageView.Width().EqualTo(64),
                avatarImageView.AtTopOf(containerView, 12),
                avatarImageView.AtBottomOf(containerView, 12),
                avatarImageView.AtLeftOf(containerView, 12),

                categoryName.WithSameCenterY(containerView),
                categoryName.ToRightOf(avatarImageView, 12),
                categoryName.AtRightOf(containerView, 12)
            );

            return containerView;
        }

        public static UIViewController Show(FilterBudgetByCategoryViewModel viewModel)
        {
            var mainViewController = UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController;
            var contentViewController = new FilterBudgetByCategoryViewController()
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
            });
            return bottomDrawerViewController;
        }
  
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ViewDidDisappear(animated);
        }
    }
}