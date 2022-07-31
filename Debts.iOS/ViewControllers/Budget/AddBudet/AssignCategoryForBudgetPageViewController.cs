using System;
using Cirrious.FluentLayouts.Touch;
using Debts.Converters;
using Debts.Data;
using Debts.iOS.Config;
using Debts.iOS.Core.VIews;
using Debts.iOS.Utilities.Extensions;
using Debts.Resources;
using Debts.ViewModel.Budget;
using Debts.ViewModel.Finances;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.Plugin.Color.Platforms.Ios;
using UIKit;

namespace Debts.iOS.ViewControllers.Finances.AddFinance
{
    public class AssignCategoryForBudgetPageViewController : MvxViewController
    {
        private UIImageView avatarImageView;
        private UILabel categoryName;

        public AssignCategoryForBudgetPageViewController()
        {
        }

        public AssignCategoryForBudgetPageViewController(NSCoder coder) : base(coder)
        {
        }

        protected AssignCategoryForBudgetPageViewController(NSObjectFlag t) : base(t)
        {
        }

        protected internal AssignCategoryForBudgetPageViewController(IntPtr handle) : base(handle)
        {
        }

        public AssignCategoryForBudgetPageViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }
  
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            UIImageView iconImageView = new UIImageView(UIImage.FromBundle("add_operation_02"))
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            
            UILabel titleLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(18, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(128,128,128),
                Text = TextResources.AddBudget_Page_2_Title,
                TextAlignment = UITextAlignment.Center
            };
            
            UILabel contentLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(162, 162, 162),
                Text = TextResources.AddBudget_Page_2_Content,
                TextAlignment = UITextAlignment.Center,
                Lines = 0
            };
            
            Add(iconImageView);
            Add(titleLabel);
            Add(contentLabel);
            var operationTypeView = PrepareContainerView();
            Add(operationTypeView);
            
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            View.AddConstraints(
                iconImageView.WithSameCenterX(View),
                iconImageView.AtTopOf(View, 24),
                iconImageView.Height().EqualTo(96),
                    
                titleLabel.AtLeftOf(View, 12),
                titleLabel.AtRightOf(View, 12),
                titleLabel.Below(iconImageView, 12),
                    
                contentLabel.Below(titleLabel, 12),
                contentLabel.AtLeftOf(View, 36),
                contentLabel.AtRightOf(View, 36),
                
                operationTypeView.Below(contentLabel, 48),
                operationTypeView.AtLeftOf(View, 16),
                operationTypeView.AtRightOf(View, 16)
            );

            var set =
                this.CreateBindingSet<AssignCategoryForBudgetPageViewController, AddBudgetViewModel>();

            set.Bind(this)
                .For(x => x.SelectedCategory)
                .To(x => x.SelectedCategory);

            set.Bind(categoryName)
                .To(x => x.SelectedCategory)
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
        
           private UIView PrepareContainerView()
          { 
             var containerView = new UIView {BackgroundColor = AppColors.GrayForFieldContainer};
             containerView.Layer.CornerRadius = 6;
             containerView.UserInteractionEnabled = true;
             containerView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
             {
                 var viewModel = ViewModel as AddBudgetViewModel;
                 viewModel.PickCategory.Execute();
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
    }
}