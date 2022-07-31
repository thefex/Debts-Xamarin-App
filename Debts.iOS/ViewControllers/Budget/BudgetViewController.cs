using System;
using System.ComponentModel;
using Cirrious.FluentLayouts.Touch;
using Debts.Converters.AppDomain;
using Debts.Data;
using Debts.iOS.Cells;
using Debts.iOS.Cells.Budget;
using Debts.iOS.Config;
using Debts.iOS.Utilities;
using Debts.iOS.Utilities.Extensions;
using Debts.iOS.Utilities.TableView;
using Debts.iOS.Utilities.TableView.Grouped;
using Debts.iOS.ViewControllers.Base;
using Debts.iOS.ViewControllers.Finances;
using Debts.Model;
using Debts.Resources;
using Debts.ViewModel.Budget;
using Debts.ViewModel.FinancesViewModel;
using Foundation;
using Google.MobileAds;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers.Budget
{
    [MvxPagePresentation]
    public class BudgetViewController : BaseListViewController<BudgetListViewModel, string, BudgetItem, BudgetItemGroup>
    {
        private BannerView _adBannerView;

        public BudgetViewController()
        {
        }

        public BudgetViewController(IntPtr handle) : base(handle)
        {
        }

        public BudgetViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            _adBannerView = new BannerView(AdSizeCons.SmartBannerPortrait)
            {
                AutoloadEnabled = true,
                RootViewController = this,
                AdUnitId = iOSAppConstants.ListHeaderAdMobId
            };

            TableView.TableHeaderView = GetHeaderView();
            var tableViewSource = TableView.Source as MvxTableViewSource;
 
            var set = this.CreateBindingSet<BudgetViewController, BaseFinancesViewModel>();
            
            set.Bind(tableViewSource)
                .For(x => x.SelectionChangedCommand)
                .To(x => x.Details); 
                        
            set.Apply();

            (ViewModel as INotifyPropertyChanged).PropertyChanged += (e, a) =>
            {
                if (a.PropertyName == nameof(ViewModel.HasDateFilter) || 
                    a.PropertyName == nameof(ViewModel.FilterDates) || 
                    a.PropertyName == nameof(ViewModel.SelectedFilterCategory) ||
                    a.PropertyName == nameof(ViewModel.AreAdsAvailable))
                    TableView.TableHeaderView = GetHeaderView();    
                
            };
            
            this.AddCloseKeyboardOnTapHandlers(x => { return x is UISearchBar; });
        }

           private UISearchBar searchBar;
        UIView GetHeaderView()
        {
            UIView headerView = new UIView()
            {
                BackgroundColor = AppColors.GrayBackground
            };
 
            UILabel titleLabel = new UILabel
            {
                Font = UIFont.SystemFontOfSize(20, UIFontWeight.Regular),
                Text = TextResources.BudgetList_Title,
                TextAlignment = UITextAlignment.Left,
                TextColor = AppColors.GrayForTextFieldContainer
            };

            var searchBarHeight = 44;

            searchBar = new UISearchBar()
            {
                BarTintColor = AppColors.GrayBackground,
                BackgroundColor = AppColors.GrayBackground,
                BackgroundImage = new UIImage(),
                Translucent = true,
                TintColor = AppColors.GrayForTextFieldContainer,
                ShowsCancelButton = false,
                AutocapitalizationType = UITextAutocapitalizationType.None,
                AutocorrectionType = UITextAutocorrectionType.Default
            };
            searchBar.SearchButtonClicked += (e, a) =>
            {
                View.Window?.EndEditing(true);
            };
            searchBar.TextChanged += (e, a) =>
            {
                ViewModel.SearchCommand.Execute(searchBar.Text);
            };
			
            headerView.Add(titleLabel);
            headerView.Add(searchBar);

            var dateConverter = new FinancesViewModelToDateRangeValueConverter();
            
            UILabel forDateLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular),
                TextAlignment = UITextAlignment.Left,
                Lines = 0,
                TextColor = AppColors.GrayForTextFieldContainer
            };

            string labelText = string.Empty;

            if (ViewModel.HasDateFilter)
                labelText = (dateConverter.Convert(ViewModel.FilterDates, null, null, null) as string);

            if (ViewModel.SelectedFilterCategory != null)
            {
                if (!string.IsNullOrEmpty(labelText))
                    labelText += Environment.NewLine;

                labelText += ViewModel.SelectedFilterCategory.Name;
            }

            forDateLabel.Text = labelText;
            
            UIButton closeImageView = new UIButton(UIButtonType.Close);  

            headerView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            headerView.AddConstraints(
                titleLabel.WithSameCenterY(searchBar),
                titleLabel.AtLeftOf(headerView, 12),
					
                searchBar.AtTopOf(headerView),
                searchBar.AtBottomOf(headerView, 6),
                searchBar.ToRightOf(titleLabel, 12),
                searchBar.AtRightOf(headerView, 12),
                searchBar.Height().EqualTo(44)
            );
            int heightConstant = 0;
            if (ViewModel.HasDateFilter || ViewModel.SelectedFilterCategory != null)
            {
                headerView.Add(forDateLabel);
                headerView.Add(closeImageView);
                
                if (ViewModel.AreAdsAvailable)
                    headerView.Add(_adBannerView);
                
                headerView.AddConstraints(
                        forDateLabel.Below(searchBar, 6),
                        forDateLabel.AtLeftOf(headerView, 12),
                        forDateLabel.ToLeftOf(closeImageView),
                        
                        closeImageView.WithSameCenterY(forDateLabel),
                        closeImageView.Height().EqualTo(32),
                        closeImageView.Width().EqualTo(32),
                        closeImageView.AtRightOf(headerView, 12)
                );

                if (ViewModel.AreAdsAvailable)
                {
                    headerView.AddConstraints(
                        _adBannerView.Below(forDateLabel),
                        _adBannerView.AtLeftOf(headerView),
                        _adBannerView.AtRightOf(headerView)
                    );
                }
                 
                closeImageView.TouchUpInside += (e,a) =>
                {
                    ViewModel.ResetFilter.Execute();
                };
                
                headerView.BringSubviewToFront(closeImageView);

                heightConstant = 32;

                if (ViewModel.HasDateFilter && ViewModel.SelectedFilterCategory != null)
                    heightConstant += 24;
                
                heightConstant += ViewModel.AreAdsAvailable ? (int)_adBannerView.Frame.Height : 0;
            }
            else if (ViewModel.AreAdsAvailable)
            {
                headerView.Add(_adBannerView);
                headerView.AddConstraints(
                    _adBannerView.Below(searchBar),
                    _adBannerView.AtLeftOf(headerView),
                    _adBannerView.AtRightOf(headerView)
                );

                heightConstant += (int) _adBannerView.Frame.Height;
            }
			
            headerView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
			
            headerView.SetNeedsDisplay();
            headerView.LayoutIfNeeded();
            var height = headerView.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize).Height;
 
            var headerFrame = headerView.Frame;
            headerFrame.Width = View.Frame.Width;
            headerFrame.Height = height + heightConstant;
            headerView.Frame = headerFrame;

            if (ViewModel.AreAdsAvailable)
            {
                var defaultRequest = Google.MobileAds.Request.GetDefaultRequest();
            //    defaultRequest.TestDevices = new [] { "ba1315325f7922619c05283abe12338e" };
                
                _adBannerView.LoadRequest(defaultRequest);
            }
                
   
            return headerView;
        }
        
        protected override bool IsGroupingSupported => true;

        protected override UITableViewStyle TableViewStyle => UITableViewStyle.InsetGrouped;
        
        public override bool HasEmptyListButton => false;

        public override string EmptyListText => TextResources.BudgetList_EmptyListText;
        public override string EmptyListAssetName => "default_empty_list";
        protected override MvxTemplateSelector GetTemplateSelector()
        {
            return new BudgetListTemplateSelector(TableView, () => ViewModel);
        }
    }
}