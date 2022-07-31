using System;
using System.Collections.Generic;
using System.Linq;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Debts.Converters;
using Debts.iOS.Config;
using Debts.iOS.ViewControllers.Base;
using Debts.iOS.ViewModels;
using Debts.Messenging;
using Debts.Messenging.Messages;
using Debts.Resources;
using Debts.ViewModel.Budget;
using Debts.ViewModel.Finances;
using DT.iOS.DatePickerDialog;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.ViewControllers.Menu
{
    public class FilterBudgetByDateRangeViewController : BaseViewController<FilterBudgetByDateViewModel, string>
    {
        private InkTouchController _inkTouchController;
        public FilterBudgetByDateRangeViewController()
        {
                
        }

        public FilterBudgetByDateRangeViewController(IntPtr ptr) : base(ptr)
        {
            
        }

        protected override IEnumerable<IMvxMessageObserver> GetMessageObservers()
        {
            var observers = base.GetMessageObservers().ToList();
            
            observers.Add(new InvokeActionMessageObserver<PickDateMvxMessage>(msg =>
            {
                string title = string.Empty;
                
                if (msg.Tag == FilterBudgetByDateViewModel.StartDatePickerTag)
                {
                    title = "Select start date";
                } else if (msg.Tag == FilterBudgetByDateViewModel.EndDatePickerTag)
                    title = "Select end date";
                
                DatePickerDialog datePickerDialog = new DatePickerDialog();
                datePickerDialog.Show(
                    title,
                    datePicked =>
                    {
                        if (msg.Tag == FilterBudgetByDateViewModel.StartDatePickerTag)
                            ViewModel.StartDate = datePicked;
                        else if (msg.Tag == FilterBudgetByDateViewModel.EndDatePickerTag)
                            ViewModel.EndDate = datePicked;
                    },
                    UIDatePickerMode.Date
                );
            }));

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
                Text = "Filter by date range",
                TextAlignment = UITextAlignment.Left,
                TextColor = AppColors.GrayForTextFieldContainer
            };

            UIView startDateView = new UIView();

            UILabel startDateHint = new UILabel()
            {
                Text = "Start date",
                TextColor =  hintColor,
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular)
            };
            UILabel startDateContent = new UILabel()
            {
                TextColor = AppColors.GrayForTextFieldContainer,
                Font = UIFont.SystemFontOfSize(15, UIFontWeight.Regular)
            };
            
            startDateView.Add(startDateHint);
            startDateView.Add(startDateContent);

            startDateView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

            startDateView.AddConstraints(
                    startDateHint.AtTopOf(startDateView),
                    startDateHint.AtLeftOf(startDateView),
                    startDateHint.AtRightOf(startDateView),
                    
                    startDateContent.Below(startDateHint, 3),
                    startDateContent.WithSameLeft(startDateHint),
                    startDateContent.WithSameRight(startDateHint),
                    startDateContent.AtBottomOf(startDateView)
                );

            startDateView.UserInteractionEnabled = true;
            startDateView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
            {
                ViewModel.SelectStartDate.Execute();
            }));

            UIView endDateView = new UIView()
            {
                UserInteractionEnabled = true
            };
            endDateView.AddGestureRecognizer(new UITapGestureRecognizer( () =>
            {
                ViewModel.SelectEndDate.Execute();
            }));
            
            UILabel endDateHint = new UILabel()
            {
                Text = "End date",
                TextColor =  hintColor,
                Font = UIFont.SystemFontOfSize(13, UIFontWeight.Regular)
            };
            UILabel endDateContent = new UILabel()
            {
                TextColor = AppColors.GrayForTextFieldContainer,
                Font = UIFont.SystemFontOfSize(15, UIFontWeight.Regular)
            };
            
            endDateView.Add(endDateHint);
            endDateView.Add(endDateContent);
            
            endDateView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            endDateView.AddConstraints(
                endDateHint.AtTopOf(endDateView),
                endDateHint.AtLeftOf(endDateView),
                endDateHint.AtRightOf(endDateView),
                    
                endDateContent.Below(endDateHint, 3),
                endDateContent.WithSameLeft(endDateHint),
                endDateContent.WithSameRight(endDateHint),
                endDateContent.AtBottomOf(endDateView)
            );
            
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
            Add(startDateView);
            Add(endDateView);
            Add(filterButton);
            
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            View.AddConstraints(
                    title.AtTopOf(View, 3),
                    title.AtLeftOf(View, 24),
                    title.AtRightOf(View, 12),
                    
                     startDateView.Below(title, 0),
                     startDateView.WithSameLeft(title),
                    startDateView.WithSameRight(title),
                    startDateView.Height().EqualTo(36),
                    
                    endDateView.Below(startDateView, 16),
                    endDateView.WithSameLeft(title),
                    endDateView.WithSameRight(title),
                    endDateView.Height().EqualTo(36),
                    
                    filterButton.Below(endDateView, 12),
                    filterButton.AtRightOf(View, 12),
                    filterButton.AtBottomOfSafeArea(View, 12)
                );

            var set = this.CreateBindingSet<FilterBudgetByDateRangeViewController, FilterBudgetByDateViewModel>();
            
            set.Bind(startDateContent)
                .To(x => x.StartDate)
                .WithConversion(new NullableDateToTextValueConverter()
                {
                    EmptyValueText = "Tap to select filter start date" 
                });

            set.Bind(endDateContent)
                .To(x => x.EndDate)
                .WithConversion(new NullableDateToTextValueConverter()
                {
                    EmptyValueText = "Tap to select filter end date"
                });

            set.Bind(filterButton)
                .To(x => x.Filter);
            
            set.Apply();
        }

        public static UIViewController Show(FilterBudgetByDateViewModel viewModel)
        {
            var mainViewController = UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController;
            var contentViewController = new FilterBudgetByDateRangeViewController()
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