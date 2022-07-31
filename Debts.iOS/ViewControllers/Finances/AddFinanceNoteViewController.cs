using System;
using System.Linq;
using Cirrious.FluentLayouts.Touch;
using Debts.Data;
using Debts.iOS.Cells.Dropdowns;
using Debts.iOS.Config;
using Debts.iOS.Core.VIews;
using Debts.iOS.Utilities.Extensions;
using Debts.iOS.ViewControllers.Base;
using Debts.iOS.ViewControllers.Finances.AddFinance;
using Debts.Model.NavigationData;
using Debts.Resources;
using Debts.ViewModel.Finances;
using Debts.ViewModel.FinancesDetails;
using Foundation;
using FPT.Framework.iOS.UI.DropDown;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers.Finances
{
    [MvxModalPresentation(ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen, ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve)]

    public class AddFinanceNoteViewController : BaseViewController<AddFinanceDetailsNoteViewModel, AddFinanceOperationNoteNavigationData>
    {
        private UITextField enterNoteTextField;
        private InkTouchController _inkTouchController;

        public AddFinanceNoteViewController()
        {
        }
  
        protected internal AddFinanceNoteViewController(IntPtr handle) : base(handle)
        {
        }

        public AddFinanceNoteViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
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
            
            UIImageView iconImageView = new UIImageView(UIImage.FromBundle("add_operation_01"))
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
            
            UILabel titleLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(18, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(128,128,128),
                Text = TextResources.AddFinanceOperationNote_Title,
                TextAlignment = UITextAlignment.Center
            };
            
            UILabel contentLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = UIColor.FromRGB(162, 162, 162),
                Text = TextResources.AddFinanceOperationNote_Body,
                TextAlignment = UITextAlignment.Center,
                Lines = 0
            };
            
            var addNoteButton = new UIButton();
            addNoteButton.Layer.CornerRadius = 18;
            addNoteButton.Layer.MasksToBounds = true;
            addNoteButton.BackgroundColor = StartViewController.Colors.Primary;
            addNoteButton.Font = UIFont.SystemFontOfSize(13, UIFontWeight.Semibold);
            addNoteButton.ContentEdgeInsets = new UIEdgeInsets(12f, 36, 12f, 36); 
            addNoteButton.SetTitle(TextResources.AddFinanceOperationNote_Title.ToUpper(), UIControlState.Normal);
            addNoteButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            addNoteButton.TouchUpInside += (e, a) =>
            {
                bool hasNoteText = !string.IsNullOrEmpty(ViewModel.Note);
                ViewModel.AddNote.Execute();
                
                if (hasNoteText)
                    ViewModel.Close.Execute();
            };
            
            _inkTouchController = new InkTouchController(addNoteButton);
            _inkTouchController.AddInkView();

            UIButton closeImageView = new UIButton(UIButtonType.Close); 
            closeImageView.TouchUpInside += ((e,a) =>
            {
                ViewModel.Close.Execute();
            });
            viewContainer.Add(closeImageView);
            viewContainer.Add(iconImageView);
            viewContainer.Add(titleLabel);
            viewContainer.Add(contentLabel);
            
            var noteView = PrepareContainerView();
            viewContainer.Add(noteView);
            
            viewContainer.Add(addNoteButton);
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
                    
                    noteView.Below(contentLabel, 24),
                    noteView.AtLeftOf(viewContainer, 16),
                    noteView.AtRightOf(viewContainer, 16),
                
                    addNoteButton.Below(noteView, 24),
                    addNoteButton.AtRightOf(viewContainer, 24),
                    addNoteButton.AtBottomOf(viewContainer, 16)
            );
            
            View.AddConstraints(
                    viewContainer.AtLeftOf(View, 24),
                    viewContainer.AtRightOf(View, 24),
                    viewContainer.WithSameCenterY(View)
                );

            var set = this.CreateBindingSet<AddFinanceNoteViewController, AddFinanceDetailsNoteViewModel>();
            
            set.Bind(enterNoteTextField)
                .For(x => x.Text)
                .To(x => x.Note);
 
            
            set.Apply();
            
            this.AddCloseKeyboardOnTapHandlers(x => x is UIImageView);
            View.BringSubviewToFront(closeImageView);
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
          private UIView PrepareContainerView()
          {
             enterNoteTextField = new UITextField
             {
                Placeholder = TextResources.ViewModel_AddFinanceOperatioNViewModel_EnterTitleHint, 
                ReturnKeyType = UIReturnKeyType.Done,
                Tag = 4,
                TextColor = AppColors.GrayForTextFieldContainer
             };

             enterNoteTextField.ShouldReturn += (e) =>
             {
                 enterNoteTextField.ResignFirstResponder(); 
                return true;
             }; 
  
             var containerView = new UIView {BackgroundColor = AppColors.GrayForFieldContainer};
             containerView.Layer.CornerRadius = 6;
             containerView.AddSubviews(enterNoteTextField);
             containerView.AddShadow();
             containerView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
             containerView.AddConstraints( 
                
                 enterNoteTextField.AtTopOf(containerView),
                 enterNoteTextField.AtLeftOf(containerView, 16),
                 enterNoteTextField.AtRightOf(containerView, 16),
                 enterNoteTextField.Height().EqualTo( 50),
                 enterNoteTextField.AtBottomOf(containerView)
                 );

             return containerView;
          }
 
    }
}