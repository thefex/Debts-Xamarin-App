using System;
using System.Linq;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Debts.iOS.Cells.Dropdowns;
using Debts.iOS.Config;
using Debts.iOS.Utilities.Extensions;
using Debts.Resources;
using Debts.ViewModel.Budget;
using Debts.ViewModel.Finances;
using DynamicData;
using Foundation;
using FPT.Framework.iOS.UI.DropDown;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace Debts.iOS.ViewControllers.Finances.AddFinance
{
    public class EnterAmountCurrencyBudgetPageViewController : MvxViewController
    {
        private UITextField enterMoneyAmount;
        private DropDown _operationPicker;
        private UILabel currencyLabel;

        public EnterAmountCurrencyBudgetPageViewController()
        {
        }

        public EnterAmountCurrencyBudgetPageViewController(NSCoder coder) : base(coder)
        {
        }

        protected EnterAmountCurrencyBudgetPageViewController(NSObjectFlag t) : base(t)
        {
        }

        protected internal EnterAmountCurrencyBudgetPageViewController(IntPtr handle) : base(handle)
        {
        }

        public EnterAmountCurrencyBudgetPageViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            UIImageView iconImageView = new UIImageView(UIImage.FromBundle("add_operation_03"))
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            
            UILabel titleLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(18, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(128,128,128),
                Text = TextResources.AddBudget_Page_3_Title,
                TextAlignment = UITextAlignment.Center
            };
            
            UILabel contentLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(162, 162, 162),
                Text = TextResources.AddBudget_Page_3_Content,
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
            
            
            var set = this.CreateBindingSet<EnterAmountCurrencyBudgetPageViewController, AddBudgetViewModel>();

            set.Bind(enterMoneyAmount)
                .For(x => x.Text)
                .To(x => x.Amount);

            set.Bind(currencyLabel)
                .For(x => x.Text)
                .To(x => x.Currency);
            
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
                var dropDownTypeCell = (DropDownTypeCell) cell;

                dropDownTypeCell.Type = value;
            };

            _operationPicker.SelectionAction += (index, value) =>
            {
                var item = (ViewModel as AddBudgetViewModel).AvailableCurrencies.FirstOrDefault(x => x.ToString() == value);
                (ViewModel as AddBudgetViewModel).Currency = item;

                _operationPicker.Hide();
            };
        }
        
          private UIView PrepareContainerView()
          {
             enterMoneyAmount = new UITextField
             {
                 ReturnKeyType = UIReturnKeyType.Done,
                Tag = 4,
                TextColor = AppColors.GrayForTextFieldContainer,
                KeyboardType = UIKeyboardType.NumberPad,
                Placeholder = TextResources.AddFinanceOperationViewModel_EnterAmountHint
             };
             
             var addBudgetViewModel = (ViewModel as AddBudgetViewModel);
             var toolbar = new UIToolbar(new CGRect(0, 0, View.Bounds.Size.Width, 30));
             var space = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
             var doneBtn = new UIBarButtonItem(TextResources.CommonText_DONE, UIBarButtonItemStyle.Done, (e, a) =>
             {
                 View?.EndEditing(true);

                 addBudgetViewModel.Next.Execute();
             });

             toolbar.SetItems(new[] {space, doneBtn}, animated: false);
             toolbar.SizeToFit();

             enterMoneyAmount.InputAccessoryView = toolbar;
             
             enterMoneyAmount.ShouldReturn += (e) =>
             {
                 enterMoneyAmount.ResignFirstResponder();

                 if (addBudgetViewModel.CurrentSubPage == 3)
                    addBudgetViewModel.Add.Execute();
                 else
                    addBudgetViewModel.Next.Execute();
                 
                return true;
             };
             var chevronDown = CreateDownArrowImageView();
             
             var separator = CreateSeparator();
             currencyLabel = new UILabel {TextColor = AppColors.GrayForTextFieldContainer, UserInteractionEnabled = true};

             PreparePicker(currencyLabel);

             currencyLabel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
             {
                View?.EndEditing(true);
                
                if (_operationPicker.DataSource?.Length == 0)
                {
                    _operationPicker.DataSource = addBudgetViewModel
                        .AvailableCurrencies
                        .Select(x => x.ToString())
                        .ToArray();
                }
                
                _operationPicker.SelectRow(addBudgetViewModel.AvailableCurrencies.IndexOf(addBudgetViewModel.Currency));
                _operationPicker.Show();
             }));


             var containerView = new UIView {BackgroundColor = AppColors.GrayForFieldContainer};
             containerView.Layer.CornerRadius = 6;
             containerView.AddSubviews(currencyLabel, chevronDown, separator, enterMoneyAmount);
             containerView.AddShadow();
             containerView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
             containerView.AddConstraints(
                 currencyLabel.AtTopOf(containerView),
                 currencyLabel.AtLeftOf(containerView, 16),
                 currencyLabel.AtRightOf(containerView, 16),
                 currencyLabel.Height().EqualTo(50),
                 
                 chevronDown.WithSameCenterY(currencyLabel),
                 chevronDown.AtRightOf(containerView, 16),
                   
                 separator.Height().EqualTo(1),
                 separator.Below(currencyLabel),
                 separator.AtLeftOf(containerView),
                 separator.AtRightOf(containerView),
                
                 enterMoneyAmount.Below(separator),
                 enterMoneyAmount.AtLeftOf(containerView, 16),
                 enterMoneyAmount.AtRightOf(containerView, 16),
                 enterMoneyAmount.Height().EqualTo( 50),
                 enterMoneyAmount.AtBottomOf(containerView)
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