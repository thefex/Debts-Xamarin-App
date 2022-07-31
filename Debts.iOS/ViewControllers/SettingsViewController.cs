using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Debts.Converters;
using Debts.iOS.Cells.Dropdowns;
using Debts.iOS.Config;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Core.VIews;
using Debts.iOS.ViewControllers.Base;
using Debts.Model;
using Debts.Resources;
using Debts.ViewModel;
using Debts.ViewModel.Finances;
using DynamicData;
using Foundation;
using FPT.Framework.iOS.UI.DropDown;
using Humanizer;
using MaterialComponents;
using MvvmCross.Base;
using MvvmCross.Binding;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers
{
    [MvxPagePresentation]
    public class SettingsViewController : BaseViewController<SettingsViewModel, string>, IUITextFieldDelegate
    {
        readonly List<InkTouchController> _inkTouchControllerList = new List<InkTouchController>();

        private CurrencyModel beforeCurrencySet;
        private TimeSpan beforeDeadlineApproaching;
        private TimeSpan beforeUpcomingApproaching;
        
        public SettingsViewController()
        {
        }

        public SettingsViewController(IntPtr handle) : base(handle)
        {
        }

        protected SettingsViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        [Export("textFieldDidEndEditing:")]
        public void EditingEnded(UITextField textField)
        {
            ViewModel.DisplayName = textField.Text;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            var generalLabel= new UIPaddingLabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = AppColors.Accent,
                Text = "General",
                TextAlignment = UITextAlignment.Left,
                Padding = new UIEdgeInsets(12, 12, 0, 12)
            };
            
            var restorePanel = new UIView();
            restorePanel.UserInteractionEnabled = true;
            
            var restoreHint = new UILabel
            {
                Font = UIFont.SystemFontOfSize(15, UIFontWeight.Regular),
                TextColor = AppColors.StrongGray,
                Text = "Restore purchases",
                UserInteractionEnabled = false
            };
            
            restorePanel.AddGestureRecognizer(new UITapGestureRecognizer(() => { ViewModel.RestorePurchase.Execute(); })); 
        
            var restoreContent = new UILabel
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = AppColors.GrayForTextFieldContainer,
                UserInteractionEnabled = false,
                Text = "Tap here if you need to restore purchased in-app products like Premium Subscription.",
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            
            restorePanel.Add(restoreHint);
            restorePanel.Add(restoreContent);            

            var currencyPanel = new UIView();
            currencyPanel.UserInteractionEnabled = true;
             
            
            var currencyHint = new UILabel
            {
                Font = UIFont.SystemFontOfSize(15, UIFontWeight.Regular),
                TextColor = AppColors.StrongGray,
                Text = "Currency (" + ViewModel.SelectedCurrency.Value + ")",
                UserInteractionEnabled = false
            };
            
            currencyPanel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                beforeCurrencySet = ViewModel.SelectedCurrency;
                ShowPicker("Select currency", ViewModel.SelectedCurrency.Currency, picker =>
                    {
                        picker.TitleProvider = item => (item as CurrencyModel).Currency;
                    
                        picker.SelectedItem = ViewModel.SelectedCurrency;
                        picker.ItemsSource = ViewModel.Currencies;
                        picker.SelectedItemChanged += (e, args) => { ViewModel.SelectedCurrency = (picker.SelectedItem as CurrencyModel); };

                    }, 
                    () => { ViewModel.SelectedCurrency = beforeCurrencySet; },
                    () =>
                    {
                    
                    });
            })); 
        
            var currencyContent = new UILabel
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = AppColors.GrayForTextFieldContainer,
                UserInteractionEnabled = false,
                Text = "Select default currency for new operations.",
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            
            currencyPanel.Add(currencyHint);
            currencyPanel.Add(currencyContent);
            
            var deviceNamePanel  = new UIView();
            deviceNamePanel.UserInteractionEnabled = true;
            deviceNamePanel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                var alert = UIAlertController.Create("Display name",
                    "Enter display name which will be used for share finance operation feature.",
                    UIAlertControllerStyle.Alert);
                
                alert.AddAction(UIAlertAction.Create("cancel", UIAlertActionStyle.Destructive, x => { }));
                alert.AddAction(UIAlertAction.Create("ok", style: UIAlertActionStyle.Default, x => {}));
                alert.AddTextField(textField =>
                {
                    textField.Placeholder = "Enter display name";
                    textField.Text = ViewModel.DisplayName;
                    textField.Delegate = this;
                });
                
                PresentViewController(alert, true, () => {});
            }));
            
            var deviceNameHint = new UILabel
            {
                Font = UIFont.SystemFontOfSize(15, UIFontWeight.Regular),
                TextColor = AppColors.StrongGray,
                Text = "Display name",
                UserInteractionEnabled = false
            };
            
        
            var deviceNameContent = new UITextField()
            {
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular),
                TextColor = AppColors.GrayForTextFieldContainer,
                UserInteractionEnabled = false,
            };
            
            deviceNamePanel.Add(deviceNameHint);
            deviceNamePanel.Add(deviceNameContent);
            
            var rateAppPanel = new UIView();
            rateAppPanel.UserInteractionEnabled = true;
            rateAppPanel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
               ViewModel.RateApp.Execute();
            }));

            var rateAppHint = new UILabel
            {
                Text = "Rate app",
                TextColor = currencyHint.TextColor,
                Font = currencyHint.Font,
                UserInteractionEnabled = false
            };
 
            var rateAppContent = new UILabel
            {
                Text = "Rate us and share your feedback so we can build better product!",
                UserInteractionEnabled = false,
                TextColor = currencyContent.TextColor,
                Font = currencyContent.Font,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap

            };
            
            rateAppPanel.Add(rateAppHint);
            rateAppPanel.Add(rateAppContent);
            
            var goPremiumPanel = new UIView();
            goPremiumPanel.UserInteractionEnabled = true;
            goPremiumPanel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ViewModel.GoPremium.Execute();
            }));

            var goPremiumHint = new UILabel
            {
                Text = "Go premium",
                UserInteractionEnabled = false,
                TextColor = currencyHint.TextColor,
                Font = currencyHint.Font
            };
 
            var goPremiumContent = new UILabel
            {
                Text = "Remove advertisements and trial limitations.",
                UserInteractionEnabled = false,
                TextColor = currencyContent.TextColor,
                Font = currencyContent.Font,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            
            goPremiumPanel.Add(goPremiumHint);
            goPremiumPanel.Add(goPremiumContent);

            var privacyPolicyPanel = new UIView();
            privacyPolicyPanel.UserInteractionEnabled = true;
            privacyPolicyPanel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ViewModel.PrivacyPolicy.Execute();
            }));

            var privacyPolicyHint = new UILabel
            {
                Text = "Privacy Policy",
                UserInteractionEnabled = false,
                TextColor = currencyHint.TextColor,
                Font = currencyHint.Font
            };
 
            var privacyPolicyContent = new UILabel
            {
                Text = "Tap to view app privacy policy.",
                UserInteractionEnabled = false,
                TextColor = currencyContent.TextColor,
                Font = currencyContent.Font,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            
            privacyPolicyPanel.Add(privacyPolicyHint);
            privacyPolicyPanel.Add(privacyPolicyContent);


            var termsOfUsagePanel = new UIView();
            termsOfUsagePanel.UserInteractionEnabled = true;
            termsOfUsagePanel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ViewModel.TermsOfUsage.Execute();
            }));

            var termsOfUsageHint = new UILabel
            {
                Text = "Terms of usage",
                UserInteractionEnabled = false,
                TextColor = currencyHint.TextColor,
                Font = currencyHint.Font
            };
 
            var termsOfUsageContent = new UILabel
            {
                Text = "Tap to view app terms of usage.",
                UserInteractionEnabled = false,
                TextColor = currencyContent.TextColor,
                Font = currencyContent.Font,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            
            termsOfUsagePanel.Add(termsOfUsageHint);
            termsOfUsagePanel.Add(termsOfUsageContent);
            
            var notificationsLabel = new UIPaddingLabel()
            {
                Font = UIFont.SystemFontOfSize(14, UIFontWeight.Regular),
                TextColor = AppColors.Accent,
                Text = "Notifications",
                TextAlignment = UITextAlignment.Left,
                Padding = new UIEdgeInsets(12, 12, 0, 12)
            };

            var deadlineApproachingPanel = new UIView();
            deadlineApproachingPanel.UserInteractionEnabled = true;
            deadlineApproachingPanel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                beforeUpcomingApproaching = ViewModel.SelectedMinimalAmountOfTimeBeforeNotifications;
                ShowPicker(string.Empty, Humanize(ViewModel.SelectedMinimalAmountOfTimeBeforeNotifications), picker =>
                    {
                        picker.TitleProvider = item => Humanize((TimeSpan)item);
                    
                        picker.SelectedItem = ViewModel.SelectedMinimalAmountOfTimeBeforeNotifications;
                        picker.ItemsSource = ViewModel.MinimalAmountOfTimeBeforeNotifications;
                        picker.SelectedItemChanged += (e, args) => { ViewModel.SelectedMinimalAmountOfTimeBeforeNotifications = 
                            ((TimeSpan)picker.SelectedItem);
                        };
                    }, 
                    () => { ViewModel.SelectedMinimalAmountOfTimeBeforeNotifications = beforeUpcomingApproaching; },
                    () =>
                    {
                    
                    });
            }));

            var deadlineApproachingHint = new UILabel
            {
                Text = "Deadline approaching",
                TextColor = currencyHint.TextColor,
                Font = currencyHint.Font,
                UserInteractionEnabled = false
            };
 
            var deadlineApproachingContent = new UILabel
            {
                Text = "Select minimal time amount left for approaching payment date notifications.",
                TextColor = currencyContent.TextColor,
                Font = currencyContent.Font,
                UserInteractionEnabled = false,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            
            deadlineApproachingPanel.Add(deadlineApproachingHint);
            deadlineApproachingPanel.Add(deadlineApproachingContent);
            
            var deadlineExceedPanel = new UIView();
            deadlineExceedPanel.UserInteractionEnabled = true;
            deadlineExceedPanel.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                beforeDeadlineApproaching = ViewModel.SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications;
                ShowPicker(string.Empty, Humanize(ViewModel.SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications), picker =>
                    {
                        picker.TitleProvider = item => Humanize((TimeSpan)item);
                    
                        picker.SelectedItem = ViewModel.SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications;
                        picker.ItemsSource = ViewModel.MinimalAmountOfTimeBeforeNotifications;
                        picker.SelectedItemChanged += (e, args) => { ViewModel.SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications = 
                            ((TimeSpan)picker.SelectedItem);
                    };
                    }, 
                    () => { ViewModel.SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications = beforeDeadlineApproaching; },
                    () =>
                    {
                    
                    });
            }));

            var deadlineExceedPanelHint = new UILabel
            {
                Text = "Deadline Exceed",
                TextColor = currencyHint.TextColor,
                Font = currencyHint.Font,
                UserInteractionEnabled = false
            };
 
            var deadlineExceedPanelContent = new UILabel
            {
                Text = "Select minimal time amount passed for payment date exceed notifications.",
                TextColor = currencyContent.TextColor,
                Font = currencyContent.Font,
                UserInteractionEnabled = false,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            
            deadlineExceedPanel.Add(deadlineExceedPanelHint);
            deadlineExceedPanel.Add(deadlineExceedPanelContent);

            UIView upcomingDebtsPanel = new UIView();
            
            var upcomingDebtsTitle = new UILabel
            {
                Text = "Upcoming Debts",
                TextColor = currencyHint.TextColor,
                Font = currencyHint.Font,
                UserInteractionEnabled = false
            };
 
            var upcomingDebtsContent = new UILabel
            {
                Text = "Debt deadline is approaching notifications.",
                TextColor = currencyContent.TextColor,
                Font = currencyContent.Font,
                UserInteractionEnabled = false,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            UISwitch upcomingDebtsSwitch = new UISwitch()
            {
                TintColor = AppColors.Accent
            };
            
            upcomingDebtsPanel.Add(upcomingDebtsTitle);
            upcomingDebtsPanel.Add(upcomingDebtsContent);
            upcomingDebtsPanel.Add(upcomingDebtsSwitch);
            
            UIView upcomingLoansPanel = new UIView();
            
            var upcomingLoansTitle = new UILabel
            {
                Text = "Upcoming Loans",
                TextColor = currencyHint.TextColor,
                Font = currencyHint.Font,
                UserInteractionEnabled = false
            };
 
            var upcomingLoansContent = new UILabel
            {
                Text = "Loan payment deadline is approaching notifications.",
                TextColor = currencyContent.TextColor,
                Font = currencyContent.Font,
                UserInteractionEnabled = false,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            UISwitch upcomingLoansSwitch = new UISwitch()
            {
                TintColor = AppColors.Accent
            };
            
            upcomingLoansPanel.Add(upcomingLoansTitle);
            upcomingLoansPanel.Add(upcomingLoansContent);
            upcomingLoansPanel.Add(upcomingLoansSwitch);
            
            UIView unpaidDebtsPanel = new UIView();
            
            var unpaidDebtsTitle = new UILabel
            {
                Text = "Unpaid Debts",
                TextColor = currencyHint.TextColor,
                Font = currencyHint.Font,
                UserInteractionEnabled = false
            };
 
            var unpaidDebtsContent = new UILabel
            {
                Text = "Debt is still not paid notifications.",
                TextColor = currencyContent.TextColor,
                Font = currencyContent.Font,
                UserInteractionEnabled = false,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            UISwitch unpaidDebtsSwitch = new UISwitch()
            {
                TintColor = AppColors.Accent
            };
            unpaidDebtsPanel.Add(unpaidDebtsTitle);
            unpaidDebtsPanel.Add(unpaidDebtsContent);
            unpaidDebtsPanel.Add(unpaidDebtsSwitch);
            
            UIView unpaidLoansPanel = new UIView();
            
            var unpaidLoansTitle = new UILabel
            {
                Text = "Unaid Loans",
                TextColor = currencyHint.TextColor,
                Font = currencyHint.Font,
                UserInteractionEnabled = false
            };
 
            var unpaidLoansContent = new UILabel
            {
                Text = "Loan is still not paid notifications.",
                TextColor = currencyContent.TextColor,
                Font = currencyContent.Font,
                UserInteractionEnabled = false,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            UISwitch unpaidLoansSwitch = new UISwitch()
            {
                TintColor = AppColors.Accent
            };
            unpaidLoansPanel.Add(unpaidLoansTitle);
            unpaidLoansPanel.Add(unpaidLoansContent);
            unpaidLoansPanel.Add(unpaidLoansSwitch);

            UIStackView stackView = new UIStackView();
            stackView.Distribution = UIStackViewDistribution.Fill;
            stackView.Axis = UILayoutConstraintAxis.Vertical;
            
            stackView.AddArrangedSubview(generalLabel);
            stackView.AddArrangedSubview(restorePanel);
            stackView.AddArrangedSubview(currencyPanel);
            stackView.AddArrangedSubview(deviceNamePanel);
            stackView.AddArrangedSubview(rateAppPanel);
            stackView.AddArrangedSubview(goPremiumPanel);
            stackView.AddArrangedSubview(privacyPolicyPanel);
            stackView.AddArrangedSubview(termsOfUsagePanel);
            stackView.AddArrangedSubview(notificationsLabel);
            stackView.AddArrangedSubview(deadlineApproachingPanel);
            stackView.AddArrangedSubview(deadlineExceedPanel);
            stackView.AddArrangedSubview(upcomingDebtsPanel);
            stackView.AddArrangedSubview(upcomingLoansPanel);
            stackView.AddArrangedSubview(unpaidDebtsPanel);
            stackView.AddArrangedSubview(unpaidLoansPanel);

            UIScrollView stackScroller = new UIScrollView()
            {
                ContentInset = new UIEdgeInsets(0, 0, 64, 0),
                
            };
            stackScroller.Add(stackView);
            Add(stackScroller);
            
            stackScroller.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            stackView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints(); 
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            
            SetupConstraintsForPanel(restorePanel, restoreHint, restoreContent, true);
            SetupConstraintsForPanel(currencyPanel, currencyHint, currencyContent);
            SetupConstraintsForPanel(deviceNamePanel, deviceNameHint, deviceNameContent);
            SetupConstraintsForPanel(rateAppPanel, rateAppHint, rateAppContent);
            SetupConstraintsForPanel(goPremiumPanel, goPremiumHint, goPremiumContent);
            SetupConstraintsForPanel(privacyPolicyPanel, privacyPolicyHint, privacyPolicyContent);
            SetupConstraintsForPanel(termsOfUsagePanel, termsOfUsageHint, termsOfUsageContent);
            SetupConstraintsForPanel(deadlineApproachingPanel, deadlineApproachingHint, deadlineApproachingContent, true);
            SetupConstraintsForPanel(deadlineExceedPanel, deadlineExceedPanelHint, deadlineExceedPanelContent);
            SetupSwitchPanel(unpaidDebtsPanel, unpaidDebtsSwitch, unpaidDebtsTitle, unpaidDebtsContent);
            SetupSwitchPanel(unpaidLoansPanel, unpaidLoansSwitch, unpaidLoansTitle, unpaidLoansContent);
            SetupSwitchPanel(upcomingDebtsPanel, upcomingDebtsSwitch, upcomingDebtsTitle, upcomingDebtsContent);
            SetupSwitchPanel(upcomingLoansPanel, upcomingLoansSwitch, upcomingLoansTitle, upcomingLoansContent);
            
            var restoreInk = new InkTouchController(restorePanel);
            restoreInk.AddInkView();
            var currencyInk = new InkTouchController(currencyPanel);
            currencyInk.AddInkView();
            var deviceInk = new InkTouchController(deviceNamePanel);
            deviceInk.AddInkView();
            var rateInk = new InkTouchController(rateAppPanel);
            rateInk.AddInkView();
            var premiumInk = new InkTouchController(goPremiumPanel);
            premiumInk.AddInkView();
            var privacyPolicyInk = new InkTouchController(privacyPolicyPanel);
            privacyPolicyInk.AddInkView();
            var termsOfUsageInk = new InkTouchController(termsOfUsagePanel);
            termsOfUsageInk.AddInkView();
            var deadlineApproachInk = new InkTouchController(deadlineApproachingPanel);
            deadlineApproachInk.AddInkView();
            var deadlineExceedInk = new InkTouchController(deadlineExceedPanel);
            deadlineExceedInk.AddInkView();
            
            _inkTouchControllerList.Add(currencyInk);
            _inkTouchControllerList.Add(deviceInk);
            _inkTouchControllerList.Add(rateInk);
            _inkTouchControllerList.Add(premiumInk);
            _inkTouchControllerList.Add(deadlineApproachInk);
            _inkTouchControllerList.Add(deadlineExceedInk);
            _inkTouchControllerList.Add(restoreInk);
            _inkTouchControllerList.Add(privacyPolicyInk);
            _inkTouchControllerList.Add(termsOfUsageInk);
              
            stackScroller.AddConstraints(
                stackView.AtTopOf(stackScroller),
                stackView.AtLeftOf(stackScroller),
                stackView.Width().EqualTo(View.Bounds.Width),
                stackView.AtBottomOf(stackScroller)
            );
            
            View.AddConstraints(
                stackScroller.AtTopOfSafeArea(View),
                stackScroller.AtLeftOf(View),
                stackScroller.AtBottomOf(View),
                stackScroller.AtRightOf(View)
            );

            var set = this.CreateBindingSet<SettingsViewController, SettingsViewModel>();

            set.Bind(deviceNameContent)
                .To(x => x.DisplayName);
            
            set.Bind(currencyHint)
                .To(x => x.SelectedCurrency)
                .WithConversion(new GenericValueConverter<CurrencyModel, string>(x => "Currency (" + x.Currency + ")"));

            set.Bind(deadlineApproachingHint)
                .To(x => x.SelectedMinimalAmountOfTimeBeforeNotifications)
                .WithConversion(new GenericValueConverter<TimeSpan, string>(x =>
                {
                    string humanized = string.Empty;
                    try
                    {
                        humanized = x.Humanize();
                    }
                    catch (Exception)
                    {
                        humanized = x.Humanize(culture: new CultureInfo("en-us"));
                    }

                    return $"Deadline Approaching ({humanized})";
                }));
            
            set.Bind(deadlineExceedPanelHint)
                .To(x => x.SelectedMinimalAmountOfTimeAfterDeadlineExceedNotifications)
                .WithConversion(new GenericValueConverter<TimeSpan, string>(x =>
                {
                    string humanized = string.Empty;
                    try
                    {
                        humanized = x.Humanize();
                    }
                    catch (Exception)
                    {
                        humanized = x.Humanize(culture: new CultureInfo("en-us"));
                    }

                    return $"Deadline Exceed ({humanized})";
                }));
            
            set.Bind(upcomingDebtsSwitch)
                .For(x => x.On)
                .To(x => x.UpcomingDebtsNotificationsEnabled);

            set.Bind(upcomingLoansSwitch)
                .For(x => x.On)
                .To(x => x.UpcomingLoansNotificationsEnabled);

            set.Bind(unpaidDebtsSwitch)
                .For(x => x.On)
                .To(x => x.UnpaidDebtsNotificationsEnabled);

            set.Bind(unpaidLoansSwitch)
                .To(x => x.UnpaidLoansNotificationsEnabled);
            
            set.Bind(goPremiumPanel)
                .For(x => x.Hidden)
                .To(x => x.IsGoPremiumEnabled)
                .WithConversion(new BooleanNegationValueConverter());
            
            set.Apply();
        }

        void SetupConstraintsForPanel(UIView panel, UILabel hint, UIView content, bool isTop = false)
        {
            panel.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            panel.AddConstraints(
                hint.AtTopOf(panel, isTop ? 12 : 12),
                hint.AtLeftOf(panel, 12),
                hint.AtRightOf(panel, 12),
                    
                content.Below(hint, 3),
                content.AtLeftOf(panel, 12),
                content.AtRightOf(panel, 12),
                content.AtBottomOf(panel, 12)
            );
        }

        void SetupSwitchPanel(UIView panel, UISwitch switchItem, UILabel title, UILabel content)
        {
            panel.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            switchItem.OnTintColor = AppColors.Accent;
            panel.AddConstraints(
                title.AtLeftOf(panel, 12),
                title.AtRightOf(panel, 12 + 48 + 12),
                title.AtTopOf(panel, 12),
                
                content.Below(title, 3),
                content.WithSameLeft(title),
                content.WithSameRight(title),
                content.AtBottomOf(panel, 12),
                
                switchItem.AtRightOf(panel, 12),
                switchItem.WithSameCenterY(panel)
            );
        }
        
        protected virtual void ShowPicker(string headerText, object selectedItem, Action<CustomMvxPickerViewModel> setupBindings, Action cancelAction,  Action doneAction)
        {
            View.Window.EndEditing(true);
            var modalPicker = new ModalPickerViewController(ModalPickerType.Custom, headerText, this)
            {
                HeaderBackgroundColor = AppColors.GrayBackground,
                HeaderTextColor = AppColors.GrayBackground,
                TransitioningDelegate = new ModalPickerTransitionDelegate(),
                ModalPresentationStyle = UIModalPresentationStyle.Custom
            };
            var mvxPickerViewModel = new CustomMvxPickerViewModel(modalPicker.PickerView) { SelectedItem = selectedItem };
            modalPicker.PickerView.Model = mvxPickerViewModel;
            modalPicker.OnModalPickerDismissed += (args, e) =>
            {
                doneAction();
            };
            modalPicker.OnModalPickerCancelled += (args, e) => { cancelAction(); };

            setupBindings(mvxPickerViewModel);

            PresentViewController(modalPicker, true, null);
        } 
        
        string Humanize(TimeSpan x){
            try
            {
                return x.Humanize();
            }
            catch (Exception)
            {
                return x.Humanize(culture: new CultureInfo("en-us"));
            }
        }     
    }
    
    
    
  
}