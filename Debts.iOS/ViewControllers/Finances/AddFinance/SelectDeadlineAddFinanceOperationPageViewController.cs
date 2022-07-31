using System;
using System.Linq;
using Cirrious.FluentLayouts.Touch;
using Debts.Converters;
using Debts.iOS.Cells.Dropdowns;
using Debts.iOS.Config;
using Debts.iOS.Utilities.Extensions;
using Debts.Resources;
using Debts.ViewModel.Finances;
using DT.iOS.DatePickerDialog;
using Foundation;
using FPT.Framework.iOS.UI.DropDown;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace Debts.iOS.ViewControllers.Finances.AddFinance
{
    public class SelectDeadlineAddFinanceOperationPageViewController : MvxViewController
    {
        private UILabel tapToSelectPaymentDeadline;

        public SelectDeadlineAddFinanceOperationPageViewController()
        {
        }

        public SelectDeadlineAddFinanceOperationPageViewController(NSCoder coder) : base(coder)
        {
        }

        protected SelectDeadlineAddFinanceOperationPageViewController(NSObjectFlag t) : base(t)
        {
        }

        protected internal SelectDeadlineAddFinanceOperationPageViewController(IntPtr handle) : base(handle)
        {
        }

        public SelectDeadlineAddFinanceOperationPageViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            UIImageView iconImageView = new UIImageView(UIImage.FromBundle("add_operation_04"))
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            
            UILabel titleLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(18, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(128,128,128),
                Text = TextResources.AddFinanceOperation_Page_4_Title,
                TextAlignment = UITextAlignment.Center
            };
            
            UILabel contentLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(162, 162, 162),
                Text = TextResources.AddFinanceOperation_Page_4_Content,
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
                .CreateBindingSet<SelectDeadlineAddFinanceOperationPageViewController, AddFinanceOperationViewModel>();

            set.Bind(tapToSelectPaymentDeadline)
                .To(x => x.Deadline)
                .WithConversion(new NullableDateToTextValueConverter() { EmptyValueText = TextResources.AddFinanceOperationViewModel_EnterDeadlineHint });
            
            set.Apply();
        }
          
       private UIView PrepareContainerView()
       { 
             tapToSelectPaymentDeadline = new UILabel {TextColor = AppColors.GrayForTextFieldContainer, UserInteractionEnabled = true};
             

             tapToSelectPaymentDeadline.AddGestureRecognizer(new UITapGestureRecognizer(() =>
             {
                View?.EndEditing(true);
                
                var addFinanceOperationViewModel = (ViewModel as AddFinanceOperationViewModel);

                DatePickerDialog datePickerDialog = new DatePickerDialog();
                datePickerDialog.Show(
                        TextResources.DateDialog_PaymentDeadline_Title,
                        datePicked => addFinanceOperationViewModel.Deadline = datePicked,
                        UIDatePickerMode.Date
                    );
             }));


             var containerView = new UIView {BackgroundColor = AppColors.GrayForFieldContainer};
             containerView.Layer.CornerRadius = 6;
             containerView.AddSubviews(tapToSelectPaymentDeadline);
             containerView.AddShadow();
             containerView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
             containerView.AddConstraints(tapToSelectPaymentDeadline.AtTopOf(containerView, 12),
                 tapToSelectPaymentDeadline.AtLeftOf(containerView, 16),
                 tapToSelectPaymentDeadline.AtRightOf(containerView, 16),
                 tapToSelectPaymentDeadline.AtBottomOf(containerView, 12)
                 );

             return containerView;
       }
    }
}