using System;
using System.Linq;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Debts.Data;
using Debts.iOS.Cells;
using Debts.iOS.Cells.Dropdowns;
using Debts.iOS.Config;
using Debts.iOS.Utilities.Extensions;
using Debts.Resources;
using Debts.ViewModel.Budget;
using Debts.ViewModel.Finances;
using DynamicData;
using FFImageLoading.Cross;
using Foundation;
using FPT.Framework.iOS.UI.DropDown;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace Debts.iOS.ViewControllers.Finances.AddFinance
{
    public class EnterBudgetNamePageViewController : MvxViewController
    {
        private UILabel operationTypeLabel;
        private DropDown _operationPicker;
        private UITextField enterTitleTextField;

        public const int OperationTypeLabelTag = 1249;
        
        public EnterBudgetNamePageViewController()
        {
        }

        public EnterBudgetNamePageViewController(NSCoder coder) : base(coder)
        {
        }
 
        protected EnterBudgetNamePageViewController(NSObjectFlag t) : base(t)
        {
        }

        protected internal EnterBudgetNamePageViewController(IntPtr handle) : base(handle)
        {
        }

        public EnterBudgetNamePageViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            UIImageView iconImageView = new UIImageView(UIImage.FromBundle("add_operation_01"))
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            
            UILabel titleLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(18, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(128,128,128),
                Text = TextResources.AddBudget_Page_1_Title,
                TextAlignment = UITextAlignment.Center
            };
            
            UILabel contentLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(162, 162, 162),
                Text = TextResources.AddBudget_Page_1_Content,
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

            var set = this
                .CreateBindingSet<EnterBudgetNamePageViewController, AddBudgetViewModel>();

            set.Bind(operationTypeLabel)
                .For(x => x.Text)
                .To(x => x.Type);

            set.Bind(enterTitleTextField)
                .For(x => x.Text)
                .To(x => x.Title);
            
            set.Apply();
        }
 
        private void PreparePicker(UIView anchor)
        {
            _operationPicker = new DropDown
            {
                AnchorView = new WeakReference<UIView>(anchor),
                DataSource = new string[0], CellType = typeof(DropDownTypeCell), CellHeight = 60
            };

            _operationPicker.CustomCellConfiguration += (index, value, cell) =>
            {
                var budgetCell = (DropDownTypeCell) cell;

                budgetCell.Type = value;
            };

            _operationPicker.SelectionAction += (index, value) =>
            {
                var item = (ViewModel as AddBudgetViewModel).AvailableTypes.FirstOrDefault(x => x.ToString() == value);
                (ViewModel as AddBudgetViewModel).Type = item;

                _operationPicker.Hide();
            };
        }
        
          private UIView PrepareContainerView()
          {
             enterTitleTextField = new UITextField
             {
                Placeholder = TextResources.AddBudgetViewModel_TitleHint, 
                ReturnKeyType = UIReturnKeyType.Done,
                Tag = 4,
                TextColor = AppColors.GrayForTextFieldContainer
             };

             enterTitleTextField.ShouldReturn += (e) =>
             {
                 enterTitleTextField.ResignFirstResponder();
                 var addBudgetViewModel = ViewModel as AddBudgetViewModel;

                 if (addBudgetViewModel.CurrentSubPage == 3)
                    addBudgetViewModel.Add.Execute();
                 else
                    addBudgetViewModel.Next.Execute();
                 
                return true;
             };
             var chevronDown = CreateDownArrowImageView();
             
             var separator = CreateSeparator();
             operationTypeLabel = new UILabel {TextColor = AppColors.GrayForTextFieldContainer, UserInteractionEnabled = true, Tag = OperationTypeLabelTag };

             PreparePicker(operationTypeLabel);

             operationTypeLabel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
             {
                View?.EndEditing(true);
                
                var addBudgetViewModel = (ViewModel as AddBudgetViewModel);
                
                if (_operationPicker.DataSource?.Length == 0)
                {
                    _operationPicker.DataSource = addBudgetViewModel
                        .AvailableTypes
                        .Select(x => x.ToString())
                        .ToArray();
                }
 
                _operationPicker.Show();
             }));


             var containerView = new UIView {BackgroundColor = AppColors.GrayForFieldContainer};
             containerView.Layer.CornerRadius = 6;
             containerView.AddSubviews(operationTypeLabel, chevronDown, separator, enterTitleTextField);
             containerView.AddShadow();
             containerView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
             containerView.AddConstraints(
                 operationTypeLabel.AtTopOf(containerView),
                 operationTypeLabel.AtLeftOf(containerView, 16),
                 operationTypeLabel.AtRightOf(containerView, 16),
                 operationTypeLabel.Height().EqualTo(50),
                 
                 chevronDown.WithSameCenterY(operationTypeLabel),
                 chevronDown.AtRightOf(containerView, 16),
                   
                 separator.Height().EqualTo(1),
                 separator.Below(operationTypeLabel),
                 separator.AtLeftOf(containerView),
                 separator.AtRightOf(containerView),
                
                 enterTitleTextField.Below(separator),
                 enterTitleTextField.AtLeftOf(containerView, 16),
                 enterTitleTextField.AtRightOf(containerView, 16),
                 enterTitleTextField.Height().EqualTo( 50),
                 enterTitleTextField.AtBottomOf(containerView)
                 );

             return containerView;
          }

          
        private static UIView CreateSeparator() => new UIView {BackgroundColor = UIColor.White};
        
        private static UIImageView CreateDownArrowImageView() => new UIImageView(UIImage
                .FromBundle("chevron_down")
                .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate))
            {ContentMode = UIViewContentMode.ScaleAspectFit, TintColor = AppColors.GrayForTextFieldContainer};
    }
}