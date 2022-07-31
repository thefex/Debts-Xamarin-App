using System;
using Cirrious.FluentLayouts.Touch;
using Debts.Converters;
using Debts.Data;
using Debts.iOS.Config;
using Debts.iOS.Core.VIews;
using Debts.iOS.Utilities.Extensions;
using Debts.Resources;
using Debts.ViewModel.Finances;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
using UIKit;

namespace Debts.iOS.ViewControllers.Finances.AddFinance
{
    public class AssignContactForAddFinancePageViewController : MvxViewController
    {
        private UIImageView avatarImageView;
        private UILabel contactName;

        public AssignContactForAddFinancePageViewController()
        {
        }

        public AssignContactForAddFinancePageViewController(NSCoder coder) : base(coder)
        {
        }

        protected AssignContactForAddFinancePageViewController(NSObjectFlag t) : base(t)
        {
        }

        protected internal AssignContactForAddFinancePageViewController(IntPtr handle) : base(handle)
        {
        }

        public AssignContactForAddFinancePageViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
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
                Text = TextResources.AddFinanceOperation_Page_2_Title,
                TextAlignment = UITextAlignment.Center
            };
            
            UILabel contentLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(162, 162, 162),
                Text = TextResources.AddFinanceOperation_Page_2_Content,
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
                this.CreateBindingSet<AssignContactForAddFinancePageViewController, AddFinanceOperationViewModel>();

            set.Bind(this)
                .For(x => x.PickedContact)
                .To(x => x.PickedContact);

            set.Bind(contactName)
                .To(x => x.PickedContact)
                .WithConversion(new PickedContactToTextValueConverter());
            
            set.Apply();
        }

        private ContactDetails _pickedContact;
        public ContactDetails PickedContact
        {
            get => _pickedContact;
            set
            { 
                _pickedContact = value;
                new AvatarGenerator().GenerateAvatar(avatarImageView, value?.ToString() ?? "?", value?.AvatarUrl, 15, 64, UIColor.White, UIColor.Gray);
            }
        }
        
           private UIView PrepareContainerView()
          { 
             var containerView = new UIView {BackgroundColor = AppColors.GrayForFieldContainer};
             containerView.Layer.CornerRadius = 6;
             containerView.UserInteractionEnabled = true;
             containerView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
             {
                 var viewModel = ViewModel as AddFinanceOperationViewModel;
                 viewModel.PickContact.Execute();
             }));

             avatarImageView = new UIImageView() {ContentMode = UIViewContentMode.ScaleAspectFit};
             contactName = new UILabel()
             {
                TextColor = AppColors.GrayForTextFieldContainer,
                Font = UIFont.SystemFontOfSize(17, UIFontWeight.Regular)
             };
             
             containerView.AddSubviews(avatarImageView, contactName);
             containerView.AddShadow();
             containerView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
             containerView.AddConstraints(
                 avatarImageView.Height().EqualTo(64),
                 avatarImageView.Width().EqualTo(64),
                 avatarImageView.AtTopOf(containerView, 12),
                 avatarImageView.AtBottomOf(containerView, 12),
                 avatarImageView.AtLeftOf(containerView, 12),

                 contactName.WithSameCenterY(containerView),
                 contactName.ToRightOf(avatarImageView, 12),
                 contactName.AtRightOf(containerView, 12)
             );

             return containerView;
          }
    }
}