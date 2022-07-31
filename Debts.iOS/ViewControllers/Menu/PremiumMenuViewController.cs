using System;
using System.Collections.Generic;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Debts.Data;
using Debts.iOS.Cells.Menu;
using Debts.iOS.Config;
using Debts.iOS.Core.TableView.Menu;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Services.Payments;
using Debts.iOS.Utilities;
using Debts.iOS.Utilities.TableView;
using Debts.iOS.ViewControllers.Base;
using Debts.iOS.ViewModels;
using Debts.Model;
using Debts.ViewModel.AppGrowth;
using Foundation;
using MaterialComponents;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using UIKit;

namespace Debts.iOS.ViewControllers.Menu
{
    public class PremiumMenuViewController : MvxViewController 
    {
        public PremiumMenuViewController()
        {
        }
 
        protected internal PremiumMenuViewController(IntPtr handle) : base(handle)
        {
        }

        public PremiumMenuViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        public override CGSize PreferredContentSize
        {
            get => new CGSize(View.Bounds.Width, 225 + UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom);
            set => base.PreferredContentSize = value;
        }

        class PremiumMenuViewModel : MvxViewModel
        {

        }

        public MvxCommand BuyLifetimeCommand { get; set; }
        
        public MvxCommand BuySubscriptionCommand { get; set; }
        
        public override void ViewDidLoad()
        {
            ViewModel = new PremiumMenuViewModel();
            base.ViewDidLoad();
          
            var tableView = new UITableView(new CGRect(), UITableViewStyle.Grouped)
            {
                SeparatorColor = UIColor.White,
                AllowsSelection = true,
                AllowsMultipleSelection = false,
                BackgroundColor = UIColor.White,
                SeparatorStyle = UITableViewCellSeparatorStyle.None,
                ContentInset = new UIEdgeInsets(0, 0, 48, 0)
            };


            var menuTableViewSource = new MvxBaseTableViewSource(tableView, this);
            menuTableViewSource.TemplateSelector = new DefaultMvxTemplateSelector(tableView, typeof(PremiumMenuItemCell));
            tableView.Source = menuTableViewSource; 

            menuTableViewSource.SelectedItemChanged += (e, a) =>
            { 
                DismissViewController(true, () =>
                {
                    var selectedItem = menuTableViewSource.SelectedItem as PremiumMenuItem;
                    
                    if (selectedItem.IsSubscription)
                        BuySubscriptionCommand?.Execute();
                    else 
                        BuyLifetimeCommand?.Execute();

                });
            };
          
            Add(tableView);
            View.BackgroundColor = UIColor.White;
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            View.AddConstraints(
                tableView.AtTopOf(View),
                tableView.AtLeftOf(View),
                tableView.AtRightOf(View),
                tableView.AtBottomOfSafeArea(View)
            );

            menuTableViewSource.ItemsSource = new List<PremiumMenuItem>()
            {
                new PremiumMenuItem(false, "royal", "Royal Premium App", "Pay once and use premium for lifetime!"),
                new PremiumMenuItem(true, "star_full", "Premium Subscription", 
                    "Pay on a monthly base for premium access!" + Environment.NewLine + Environment.NewLine +
                    "Super Budget for Debts & Loans, duration of 1 month, price " + AppStoreBillingService.PremiumSubscriptionPrice + ", auto-renewing subscription unless explicitly canceled"
                    )
            };
        }

        public class PremiumMenuItem
        {
            public PremiumMenuItem(bool isSubscription, string icon, string title, string subtitle)
            {
                IsSubscription = isSubscription;
                Icon = icon;
                Title = title;
                Subtitle = subtitle;
            }

            public bool IsSubscription { get; set; }
            public string Icon { get; }
            public string Title { get; }
            public string Subtitle { get; }

            public bool IsLifetime => !IsSubscription;
        }

        public static void Show(GoPremiumViewModel goPremiumViewModel)
        {
            var mainViewController = UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController;
            var contentViewController = new PremiumMenuViewController()
            {
                BuyLifetimeCommand = goPremiumViewModel.BuyApp,
                BuySubscriptionCommand = goPremiumViewModel.BuySubscription
            };

            var bottomDrawerViewController = new BottomDrawerViewController();
            bottomDrawerViewController.SetTopCornersRadius(24, BottomDrawerState.Collapsed);
            bottomDrawerViewController.SetTopCornersRadius(24, BottomDrawerState.Expanded);
            bottomDrawerViewController.TopHandleHidden = false;
            bottomDrawerViewController.TopHandleColor = UIColor.LightGray;
            bottomDrawerViewController.ContentViewController = contentViewController;
 
            mainViewController.PresentedViewController.PresentViewController(bottomDrawerViewController, animated: true, () =>
            { 
                bottomDrawerViewController.SetContentOffsetY(-(mainViewController.View.Bounds.Height-contentViewController.PreferredContentSize.Height), true);
            });
        }
 
        
    }
}