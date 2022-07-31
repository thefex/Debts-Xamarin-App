using System;
using System.ComponentModel;
using Cirrious.FluentLayouts.Touch;
using Debts.Data;
using Debts.iOS.Config;
using Debts.iOS.Utilities;
using Debts.iOS.Utilities.Extensions;
using Debts.iOS.Utilities.TableView.Grouped;
using Debts.iOS.ViewControllers.Base;
using Debts.Model;
using Debts.Resources;
using Debts.ViewModel.Contacts;
using Foundation;
using Google.MobileAds;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using UIKit;

namespace Debts.iOS.ViewControllers.Contacts
{
    public abstract class BaseContactsViewController<TContactsViewModel> : BaseListViewController<TContactsViewModel, string, ContactDetails, ContactsGroup>
        where TContactsViewModel : ContactListViewModel
    {
        protected BannerView _adBannerView;
        
        public BaseContactsViewController()
        {
        }

        public BaseContactsViewController(IntPtr handle) : base(handle)
        {
        }

        public BaseContactsViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
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
            
            var set = this.CreateBindingSet<BaseContactsViewController<TContactsViewModel>, TContactsViewModel>();

            TableView.TableHeaderView = GetHeaderView();
            var tableViewSource = TableView.Source as MvxTableViewSource;
            
            set.Bind(tableViewSource)
                .For(x => x.SelectionChangedCommand)
                .To(x => x.ChildItemTapped); 
                        
            set.Apply();
            
            (ViewModel as INotifyPropertyChanged).PropertyChanged += (e, a) =>
            {
                if (a.PropertyName == nameof(ViewModel.AreAdsAvailable))
                    TableView.TableHeaderView = GetHeaderView();
            };

            this.AddCloseKeyboardOnTapHandlers(x => { return x is UISearchBar; }); 
        }

        protected override bool IsGroupingSupported => true;

        protected override UITableViewStyle TableViewStyle => UITableViewStyle.InsetGrouped;

        protected override int HeightOfAnimation => ViewModel.AreAdsAvailable ? 200 : 264;

        protected override float ScaleFactor => 0.45f;

        public override bool HasEmptyListButton => true;

        protected override string EmptyListButtonText => TextResources.ContactListViewModel_ImportContacts;

        public override string EmptyListText => TextResources.ContactListViewModel_EmptyListText;
        public override string EmptyListAssetName => "sad_bear_empty_list";

        protected override Action EmptyListButtonAction => () => ViewModel.ImportContacts.Execute();

        protected override MvxTemplateSelector GetTemplateSelector()
            => new ContactListTemplateSelector(TableView, () => ViewModel);

        protected UISearchBar searchBar;

        protected virtual UIView GetHeaderView()
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

            int heightConstant = 0;
            headerView.Add(titleLabel);
            headerView.Add(searchBar);
            if (ViewModel.AreAdsAvailable)
                headerView.Add(_adBannerView);
            
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

            if (ViewModel.AreAdsAvailable)
            {
                headerView.AddConstraints(
                    _adBannerView.Below(searchBar),
                    _adBannerView.AtLeftOf(headerView),
                    _adBannerView.AtRightOf(headerView)
                );
                heightConstant = (int)_adBannerView.Frame.Height;
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
                _adBannerView.LoadRequest(Google.MobileAds.Request.GetDefaultRequest());
            
            return headerView;
        }
        
        protected abstract string PageTitle { get; }
    }
}