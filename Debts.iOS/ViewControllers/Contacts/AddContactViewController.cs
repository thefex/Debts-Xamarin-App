using System;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Debts.Converters;
using Debts.iOS.Config;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Core.VIews;
using Debts.iOS.Utilities.Extensions;
using Debts.iOS.ViewControllers.Base;
using Debts.Model.NavigationData;
using Debts.Resources;
using Debts.ViewModel.Contacts;
using Debts.ViewModel.Finances;
using Foundation;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers.Contacts
{
    [MvxModalPresentation(ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen, ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve)]
    public class AddContactViewController : BaseViewController<AddContactViewModel, string>
    {
        private InkTouchController _buttonInkView;

        public AddContactViewController()
        {
        }

        public AddContactViewController(IntPtr handle) : base(handle)
        {
        }

        protected AddContactViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
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
            
            addButton = new UIButton();
            addButton.Layer.CornerRadius = 18;
            addButton.Layer.MasksToBounds = true;
            addButton.BackgroundColor = StartViewController.Colors.Primary;
            addButton.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold);
            addButton.ContentEdgeInsets = new UIEdgeInsets(12f, 36, 12f, 36); 
            addButton.SetTitle(TextResources.AddContactViewModel_AddContactHint, UIControlState.Normal);
            addButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            
            UIImageView iconImageView = new UIImageView(UIImage.FromBundle("add_operation_02"))
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            
            UILabel titleLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(18, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(128,128,128),
                Text = TextResources.AddContactViewModel_Title,
                TextAlignment = UITextAlignment.Center
            };
            
            UILabel contentLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(162, 162, 162),
                Text = TextResources.AddContactViewModel_Content,
                TextAlignment = UITextAlignment.Center,
                Lines = 0
            };
            
            viewContainer.Add(iconImageView);
            viewContainer.Add(titleLabel);
            viewContainer.Add(contentLabel);

            _avatarImageView = new UIImageView {UserInteractionEnabled = true, ContentMode = UIViewContentMode.ScaleAspectFit };
            _avatarImageView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ViewModel.PickPhoto.Execute();
            }));
            viewContainer.Add(_avatarImageView);

            var containerView = PrepareContainerView();
            viewContainer.Add(containerView);
            
            UIButton closeImageView = new UIButton(UIButtonType.Close); 
            closeImageView.TouchUpInside += ((e,a) =>
            {
                ViewModel.Close.Execute();
            });
            viewContainer.Add(closeImageView); 
            viewContainer.Add(addButton);
            
            addButton.TouchUpInside += (e, a) =>
            {
                ViewModel.AddContact.Execute();
            };

            _buttonInkView = new InkTouchController(addButton);
            _buttonInkView.AddInkView();
 
            Add(viewContainer);
            viewContainer.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            viewContainer.AddConstraints( 
                closeImageView.AtTopOf(viewContainer, 12),
                closeImageView.AtRightOf(viewContainer, 12),
                
                iconImageView.WithSameCenterX(viewContainer),
                iconImageView.AtTopOf(viewContainer, 24),
                iconImageView.Height().EqualTo(96),
                    
                titleLabel.AtLeftOf(viewContainer, 12),
                titleLabel.AtRightOf(viewContainer, 12),
                titleLabel.Below(iconImageView, 12),
                    
                contentLabel.Below(titleLabel, 12),
                contentLabel.AtLeftOf(viewContainer, 36),
                contentLabel.AtRightOf(viewContainer, 36),
                
                _avatarImageView.Below(contentLabel, 48),
                _avatarImageView.AtLeftOf(viewContainer, 16),
                _avatarImageView.Height().EqualTo(64),
                _avatarImageView.Width().EqualTo(64),
                
                containerView.WithSameCenterY(_avatarImageView),
                containerView.ToRightOf(_avatarImageView, 16),
                containerView.AtRightOf(viewContainer, 16),
                
                addButton.AtBottomOf(viewContainer, 24),
                addButton.WithSameCenterX(viewContainer)
            );
            
            View.AddConstraints(
                viewContainer.AtLeftOf(View, 12),
                viewContainer.AtRightOf(View, 12),
                viewContainer.Height().EqualTo(500),
                viewContainer.WithSameCenterY(View)
                );
            
            var set = this.CreateBindingSet<AddContactViewController, AddContactViewModel>();

            set.Bind(enterContactNameTextField)
                .To(x => x.ContactName);

            set.Bind(enterPhoneField)
                .To(x => x.PhoneNumber);

            set.Bind(this)
                .For(x => x.ContactName)
                .To(x => x.ContactName);

            set.Bind(this)
                .For(x => x.PicturePath)
                .To(x => x.PhotoPath);

            set.Apply();
            
            this.AddCloseKeyboardOnTapHandlers(x => x is UIImageView);
            View.BringSubviewToFront(closeImageView);
        }

        private string _contactName;
        public string ContactName
        {
            get => _contactName;
            set
            {
                _contactName = string.IsNullOrEmpty(value) ? "?" : value;
                if (string.IsNullOrEmpty(PicturePath))
                    new AvatarGenerator().GenerateAvatar(_avatarImageView, _contactName, ViewModel.PhotoPath, 15, 64, UIColor.White, UIColor.Gray);
            }
        }

        private string _picturePath;
        public string PicturePath
        {
            get => _picturePath;
            set
            {
                _picturePath = value;
                new AvatarGenerator().GenerateAvatar(_avatarImageView, string.IsNullOrEmpty(ViewModel.ContactName) ? "?" : ViewModel.ContactName, value, 15, 64, UIColor.White, UIColor.Gray);
            }
        }

        private int _pageIndex;
        private UIButton addButton;
        private UITextField enterContactNameTextField;
        private UITextField enterPhoneField;
        private UIImageView _avatarImageView;

        private UIView PrepareContainerView()
          {
             enterContactNameTextField = new UITextField
             {
                Placeholder = TextResources.AddContactViewModel_NameHint, 
                ReturnKeyType = UIReturnKeyType.Next,
                Tag = 4,
                TextColor = AppColors.GrayForTextFieldContainer
             };

             enterContactNameTextField.ShouldReturn += (e) =>
             {
                 enterPhoneField.BecomeFirstResponder();
                 return true;
             };
             
             var separator = CreateSeparator();
             enterPhoneField = new UITextField
             {
                 Placeholder = TextResources.AddContactViewModel_PhoneNumberHint, 
                 ReturnKeyType = UIReturnKeyType.Done,
                 KeyboardType = UIKeyboardType.PhonePad,
                 Tag = 4,
                 TextColor = AppColors.GrayForTextFieldContainer
             };

             enterPhoneField.ShouldReturn += (e) =>
             {
                 enterPhoneField.ResignFirstResponder();
                 return true;
             };
             
             var toolbar = new UIToolbar(new CGRect(0, 0, View.Bounds.Size.Width, 30));
             var space = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
             var doneBtn = new UIBarButtonItem(TextResources.CommonText_DONE, UIBarButtonItemStyle.Done, (e, a) =>
             {
                 View?.EndEditing(true);
             });

             toolbar.SetItems(new[] {space, doneBtn}, animated: false);
             toolbar.SizeToFit();

             enterPhoneField.InputAccessoryView = toolbar;
               
             var containerView = new UIView {BackgroundColor = AppColors.GrayForFieldContainer};
             containerView.Layer.CornerRadius = 6;
             containerView.AddSubviews(enterContactNameTextField, separator, enterPhoneField);
             containerView.AddShadow();
             containerView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
             containerView.AddConstraints(
                 enterContactNameTextField.AtTopOf(containerView),
                 enterContactNameTextField.AtLeftOf(containerView, 16),
                 enterContactNameTextField.AtRightOf(containerView, 16),
                 enterContactNameTextField.Height().EqualTo(50),
                   
                 separator.Height().EqualTo(1),
                 separator.Below(enterContactNameTextField),
                 separator.AtLeftOf(containerView),
                 separator.AtRightOf(containerView),
                
                 enterPhoneField.Below(separator),
                 enterPhoneField.AtLeftOf(containerView, 16),
                 enterPhoneField.AtRightOf(containerView, 16),
                 enterPhoneField.Height().EqualTo( 50),
                 enterPhoneField.AtBottomOf(containerView)
                 );

             return containerView;
          }

          
        private static UIView CreateSeparator() => new UIView {BackgroundColor = UIColor.White};
    }
     
}