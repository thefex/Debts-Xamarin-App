using System;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Debts.Converters.AppDomain;
using Debts.Data;
using Debts.iOS.Cells.Menu;
using Debts.iOS.Config;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.ViewControllers.Base;
using Debts.iOS.ViewModels;
using Debts.Model;
using Foundation;
using FPT.Framework.iOS.UI.DropDown;
using MaterialComponents;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers.Menu
{
   public class MenuViewController: BaseViewController<MenuViewModel, SelectedSubPage> 
   {
      public MenuViewController()
      {
      }
 
      protected internal MenuViewController(IntPtr handle) : base(handle)
      {
      }

      public MenuViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
      {
      }

      public override CGSize PreferredContentSize
      {
          get => new CGSize(View.Bounds.Width, 506+ UIApplication.SharedApplication.KeyWindow.SafeAreaInsets.Bottom);
          set => base.PreferredContentSize = value;
      }

      public override void ViewDidLoad()
      {
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


          var menuTableViewSource = new MenuTableViewSource(tableView);
          tableView.Source = menuTableViewSource;
          tableView.TableHeaderView = BuildHeaderView();
          //tableView.TableHeaderView = new UIView(new CGRect(0, 0, View.Frame.Width, 24));

          menuTableViewSource.SelectedItemChanged += (e, a) =>
          { 
              DismissViewController(true, () =>
              {
                  var selectedItem = menuTableViewSource.SelectedItem as MenuViewModel.MenuItem;
                  var mainViewController = UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController;

                  var parentViewController = ParentViewController.PresentingViewController as MainViewController;
                  var viewModel = mainViewController.ViewModel;
                  switch (selectedItem.PageType)
                  {
                      case SelectedSubPage.All:
                          viewModel.AllFinances.Execute();
                          break;
                      case SelectedSubPage.Debts:
                          viewModel.Debts.Execute();
                          break;
                      case SelectedSubPage.Loans:
                          viewModel.Loans.Execute();
                          break;
                      case SelectedSubPage.FavoritesFinances:
                          viewModel.FavoritesFinances.Execute();
                          break;
                      case SelectedSubPage.Budget:
                          viewModel.Budget.Execute();
                          break;
                      case SelectedSubPage.Contacts:
                          viewModel.Contacts.Execute();
                          break;
                      case SelectedSubPage.Statistics:
                          viewModel.Statistics.Execute();
                          break;
                      case SelectedSubPage.Settings:
                          viewModel.Settings.Execute();
                          break;
                  }
              });
          };
          
          Add(tableView);
          View.BackgroundColor = UIColor.White;
          View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
          View.AddConstraints(
                tableView.AtTopOf(View),
                tableView.AtLeftOf(View),
                tableView.AtRightOf(View),
                tableView.AtBottomOf(View)
              );

          var set = this.CreateBindingSet<MenuViewController, MenuViewModel>();

          set.Bind(menuTableViewSource)
              .For(x => x.ItemsSource)
              .To(x => x.Groups);    
          
          set.Apply();
      }

      public static void Show()
      {
          var mainViewController = UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController;
          var contentViewController = new MenuViewController()
          {
              ViewModel = Mvx.IoCProvider.Resolve<MenuViewModel>()
          };
          contentViewController.ViewModel.Prepare(mainViewController.ViewModel.SelectedSubPage);

          var bottomDrawerViewController = new BottomDrawerViewController();
          bottomDrawerViewController.SetTopCornersRadius(24, BottomDrawerState.Collapsed);
          bottomDrawerViewController.SetTopCornersRadius(24, BottomDrawerState.Expanded);
          bottomDrawerViewController.TopHandleHidden = false;
          bottomDrawerViewController.TopHandleColor = UIColor.LightGray;
          bottomDrawerViewController.ContentViewController = contentViewController;
 
          mainViewController.PresentViewController(bottomDrawerViewController, animated: true, () =>
          {
              bottomDrawerViewController.SetContentOffsetY(
                  -(mainViewController.View.Bounds.Height - contentViewController.PreferredContentSize.Height), true);
          });
      }

      UIView BuildHeaderView()
      {
          UIView header = new UIView();
          MenuViewModelToPremiumStateTextValueConverter premiumStateTextValueConverter = new MenuViewModelToPremiumStateTextValueConverter();

          string imageName = "star_empty";

          var menuViewModel = ViewModel as MenuViewModel;
          switch (menuViewModel.PremiumState)
          {
              case PremiumState.OneDayExtendedTrial:
                  imageName = "star_half";
                  break;
              case PremiumState.PremiumSubscription:
                  imageName = "star_full";
                  break;
              case PremiumState.PremiumOwnership:
                  imageName = "royal";
                  break;
          }


          UIImageView iconView = new UIImageView()
          {
              Image = UIImage.FromBundle(imageName).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate),
              TintColor = AppColors.AppInfoOrange,
              ContentMode = UIViewContentMode.ScaleAspectFit
          };
          
          
          UILabel titleLabel = new UILabel
          {
              Font = UIFont.SystemFontOfSize(18, UIFontWeight.Regular),
              TextAlignment = UITextAlignment.Left,
              Text = premiumStateTextValueConverter.Convert(ViewModel, null, null, null) as string,
              TextColor = AppColors.GrayForTextFieldContainer,
              Lines = 2,
              LineBreakMode = UILineBreakMode.WordWrap
          };
          
          header.Add(iconView);
          header.Add(titleLabel);
          
          header.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
          header.AddConstraints(
            iconView.AtLeftOf(header, 24),
            iconView.WithSameCenterY(titleLabel), 
            
            titleLabel.ToRightOf(iconView, 12),
            titleLabel.AtTopOf(header, 18),
            titleLabel.AtRightOf(header, 12),
            titleLabel.AtBottomOf(header, 12)
          );

          header.UserInteractionEnabled = true;
          header.AddGestureRecognizer(new UITapGestureRecognizer(() =>
          {
              DismissViewController(true, () =>
              {
                  ViewModel.GoPremium.Execute(); 
              });
          }));
          	
          header.SetNeedsDisplay();
          header.LayoutIfNeeded();
          var height = header.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize).Height;
          var frame = View.Frame;
          frame.Height = height;
          header.Frame = frame;

          return header;
      }
   }
}