using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Debts.Data;
using Debts.Droid.Services.Walkthrough;
using Debts.iOS.Config;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Utilities;
using Debts.iOS.Utilities.TableView.Grouped;
using Debts.iOS.ViewControllers.Base;
using Debts.Model.Sections;
using Debts.Resources;
using Debts.Services.Settings;
using Debts.ViewModel;
using FFImageLoading.Extensions;
using FFImageLoading.Work;
using Foundation;
using FPT.Framework.iOS.UI.DropDown;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers.Finances
{
    [MvxPagePresentation]
    public class FinanceDetailsViewController : BaseViewController<FinanceDetailsViewModel, FinanceOperation>
    {
        private UIButton favoriteButton;

        public FinanceDetailsViewController()
        {
        }

        public FinanceDetailsViewController(IntPtr handle) : base(handle)
        {
        }

        protected FinanceDetailsViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        }

        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = AppColors.GrayBackground;
            UIView topBar = new UIView()
            {
                BackgroundColor = AppColors.GrayBackground
            };
            
            UIButton backButton = new UIButton(UIButtonType.Custom);
            backButton.SetImage(UIImage.GetSystemImage("arrow.left").ResizeUIImage(24, 24, InterpolationMode.High).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            backButton.TintColor = AppColors.GrayForTextFieldContainer;
            
            favoriteButton = new UIButton(UIButtonType.Custom);
            favoriteButton.SetImage(UIImage.FromBundle("star_empty").ResizeUIImage(24, 24, InterpolationMode.High).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            favoriteButton.TintColor = AppColors.GrayForTextFieldContainer;
            
            UIButton deleteButton = new UIButton(UIButtonType.Custom);
            deleteButton.SetImage(UIImage.GetSystemImage("trash.fill").ResizeUIImage(24, 24, InterpolationMode.High).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            deleteButton.TintColor = AppColors.GrayForTextFieldContainer;
            
            UITableView tableView = new UITableView(new CGRect(), UITableViewStyle.InsetGrouped)
            {
                SeparatorColor = AppColors.GrayBackground,
                AllowsSelection = true,
                AllowsMultipleSelection = false,
                BackgroundColor = AppColors.GrayBackground,
                SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine,
                ContentInset = new UIEdgeInsets(0, 0, 82, 0)
            };
            var groupedTableViewSource =  new MvxGroupedBaseTableViewSource(tableView, this);
            groupedTableViewSource.ParentViewControllerProvider = () => this;
            groupedTableViewSource.TemplateSelector = new FinanceDetailsSectionTemplateSelector(tableView);
            groupedTableViewSource.ReloadOnAllItemsSourceSets = true;
            tableView.Source = groupedTableViewSource;
            
            topBar.Add(backButton);
            topBar.Add(favoriteButton);
            topBar.Add(deleteButton);
            
            Add(topBar);
            Add(tableView);

            topBar.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            
            topBar.AddConstraints(
                    backButton.AtTopOf(topBar, 12),
                    backButton.AtLeftOf(topBar, 12),
                    backButton.AtBottomOf(topBar, 12),
                    
                    deleteButton.AtRightOf(topBar, 12),
                    deleteButton.WithSameTop(backButton),
                    deleteButton.WithSameBottom(backButton),
                    
                    favoriteButton.ToLeftOf(deleteButton, 12),
                    favoriteButton.WithSameTop(deleteButton),
                    favoriteButton.WithSameBottom(deleteButton)
                );
            
            View.AddConstraints(
                    topBar.AtTopOfSafeArea(View),
                    topBar.AtLeftOf(View),
                    topBar.AtRightOf(View),
                    
                    tableView.Below(topBar),
                    tableView.AtLeftOf(View),
                    tableView.AtRightOf(View),
                    tableView.AtBottomOf(View)
                );

            var set = this.CreateBindingSet<FinanceDetailsViewController, FinanceDetailsViewModel>();

            set.Bind(backButton)
                .To(x => x.Close);

            set.Bind(deleteButton)
                .To(x => x.Delete);

            set.Bind(favoriteButton)
                .To(x => x.ToggleFavourite);

            set.Bind(this)
                .For(x => x.IsFavourite)
                .To(x => x.IsFavourite);

            set.Bind(this)
                .For(x => x.IsPaid)
                .To(x => x.IsPaid);
            
            set.Bind(groupedTableViewSource)
                .For(x => x.ItemsSource)
                .To(x => x.Sections)
                .WithConversion(new GenericValueConverter<List<DetailsSection>, List<DetailsSection>>(sections =>
                {
                    var convertedSections = new List<DetailsSection>(sections);
                    convertedSections.Insert(0, new FinanceDetailsHeaderSection() 
                    { 
                        Title = TextResources.FinanceDetailsViewModel_General,
                        DataContext = ViewModel
                    });
                    convertedSections.Insert(1, new FinanceDetailsFooterSection()
                    {
                        Title = TextResources.FinanceDetailsViewModel_Details,
                        DataContext = ViewModel
                    });

                    return convertedSections;
                }));
            
            set.Apply();
            
            FinanceDetailsWalkthroughService financeDetailsWalkthroughService = new FinanceDetailsWalkthroughService(Mvx.IoCProvider.Resolve<WalkthroughService>());
            financeDetailsWalkthroughService.Initialize(ViewModel.IsPaid, ViewModel.ArePhoneRelatedFeaturesEnabled);

            await Task.Delay(500);
            financeDetailsWalkthroughService.ShowIfPossible(this, favoriteButton, deleteButton, ViewModel.ArePhoneRelatedFeaturesEnabled, !ViewModel.IsPaid);
        }
  
        public bool IsFavourite
        {
            get => ViewModel.IsFavourite;
            set
            {
                string assetName = value ? "star_full" : "star_empty";
                favoriteButton.SetImage(UIImage.FromBundle(assetName).ResizeUIImage(32, 32, InterpolationMode.High).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            }
        }

        private bool _isPaid;
        public bool IsPaid
        {
            get => _isPaid;
            set
            {
                if (_isPaid != value)
                {
                    _isPaid = value;
                    (UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController).BottomAppBarView.SetFloatingButtonHidden(true, true);
                }
            }
        }
    }
}