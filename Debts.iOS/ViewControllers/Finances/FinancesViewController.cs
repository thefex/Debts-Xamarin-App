using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Cirrious.FluentLayouts.Touch;
using Debts.Converters.AppDomain;
using Debts.Data;
using Debts.Droid.Services.Walkthrough;
using Debts.iOS.Cells;
using Debts.iOS.Config;
using Debts.iOS.Utilities;
using Debts.iOS.Utilities.Extensions;
using Debts.iOS.Utilities.TableView;
using Debts.iOS.Utilities.TableView.Grouped;
using Debts.iOS.ViewControllers.Base;
using Debts.Model;
using Debts.Resources;
using Debts.Services.Settings;
using Debts.ViewModel.FinancesViewModel;
using Foundation;
using FPT.Framework.iOS.UI.DropDown;
using Google.MobileAds;
using Google.MobileAds.DoubleClick;
using iAd;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;
using BannerView = Google.MobileAds.BannerView;
using FinancesOperationGroup = Debts.Model.FinancesOperationGroup;

namespace Debts.iOS.ViewControllers.Finances
{
    public abstract class FinancesViewController<TViewModel> : BaseListViewController<TViewModel, string,  FinanceOperation, FinancesOperationGroup>
        where TViewModel : BaseFinancesViewModel
    {
        private BannerView _adBannerView;
        
        public FinancesViewController()
        {
        }

        public FinancesViewController(IntPtr handle) : base(handle)
        {
        }

        protected FinancesViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        public override async void ViewDidLoad()
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
 
            var set = this.CreateBindingSet<FinancesViewController<TViewModel>, BaseFinancesViewModel>();
            
            set.Bind(tableViewSource)
                .For(x => x.SelectionChangedCommand)
                .To(x => x.Details); 
                        
            set.Apply();

            (ViewModel as INotifyPropertyChanged).PropertyChanged += (e, a) =>
            {
                if (a.PropertyName == nameof(ViewModel.HasDateFilter) || a.PropertyName == nameof(ViewModel.FilterDates) || a.PropertyName == nameof(ViewModel.AreAdsAvailable))
                    TableView.TableHeaderView = GetHeaderView();    
                
            };
            
            this.AddCloseKeyboardOnTapHandlers(x => { return x is UISearchBar; });
            
            MainWalkthroughService mainWalkthroughService = new MainWalkthroughService(Mvx.IoCProvider.Resolve<WalkthroughService>());
            await Task.Delay(500);
            
            mainWalkthroughService.ShowIfPossible(this);  
        }

        public override bool HasEmptyListButton => false;
        
        public override string EmptyListAssetName => "default_empty_list";
        
        protected override bool IsGroupingSupported => true;

        protected override UITableViewStyle TableViewStyle => UITableViewStyle.InsetGrouped;

        protected override int HeightOfAnimation => 152;

        protected override float ScaleFactor => 0.45f;
        
        protected override MvxTemplateSelector GetTemplateSelector()
            => new FinanceListTemplateSelector(TableView, () => ViewModel);

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
                Text = PageTitle,
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
                Text = dateConverter.Convert(ViewModel.FilterDates, null, null, null) as string,
                TextAlignment = UITextAlignment.Left,
                TextColor = AppColors.GrayForTextFieldContainer
            };
            
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
            if (ViewModel.HasDateFilter)
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
                        closeImageView.Width().GreaterThanOrEqualTo(24),
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
                    ViewModel.ResetDateFilter.Execute();
                };

                headerView.BringSubviewToFront(closeImageView);
                heightConstant = 32;
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
        
        protected abstract string PageTitle { get; }
    }
}