using System;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.Resources;
using Debts.ViewModel.Contacts;
using Foundation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers.Contacts
{
    [MvxModalPresentation(Animated = true, ModalPresentationStyle = UIModalPresentationStyle.OverFullScreen, ModalTransitionStyle = UIModalTransitionStyle.CoverVertical)]
    public class PickContactViewController : BaseContactsViewController<PickContactViewModel>
    {
        public PickContactViewController()
        {
        }

        public PickContactViewController(IntPtr handle) : base(handle)
        {
        }

        public PickContactViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        protected override UIView GetHeaderView()
        {
            UIView headerView = new UIView()
            {
                BackgroundColor = AppColors.GrayBackground
            };

            UIButton closeImageView = new UIButton(UIButtonType.Close); 
            closeImageView.TouchUpInside += ((e,a) =>
            {
                ViewModel.Close.Execute();
            });
		 
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
            headerView.Add(closeImageView);
            headerView.Add(titleLabel);
            headerView.Add(searchBar);
            if (ViewModel.AreAdsAvailable)
                headerView.Add(_adBannerView);
            
            headerView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            headerView.AddConstraints(
                closeImageView.AtLeftOf(headerView, 12),
                closeImageView.WithSameCenterY(titleLabel),
                
                titleLabel.WithSameCenterY(searchBar),
                titleLabel.ToRightOf(closeImageView, 6),
					
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
            
            headerView.BringSubviewToFront(closeImageView);
            return headerView;
        }
        
        public override string EmptyListText => TextResources.PickContactViewModel_EmptyListText;
        
        protected override string PageTitle => TextResources.PickContact_Title;
    }
}