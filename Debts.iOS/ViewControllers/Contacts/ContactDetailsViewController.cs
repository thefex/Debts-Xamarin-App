using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using Debts.iOS.ViewControllers.Finances;
using Debts.iOS.ViewControllers.Menu;
using Debts.Model.Sections;
using Debts.Resources;
using Debts.Services.Settings;
using Debts.ViewModel;
using DynamicData;
using FFImageLoading.Extensions;
using FFImageLoading.Work;
using Foundation;
using MaterialComponents;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers.Contacts
{
    [MvxPagePresentation]
    public class ContactDetailsViewController : BaseViewController<ContactDetailsViewModel, ContactDetails>
    {
        public ContactDetailsViewController()
        {
        }

        public ContactDetailsViewController(IntPtr handle) : base(handle)
        {
        }

        protected ContactDetailsViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
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
            groupedTableViewSource.TemplateSelector = new ContactDetailsSectionTemplateSelector(tableView);
            groupedTableViewSource.ReloadOnAllItemsSourceSets = true;
            tableView.Source = groupedTableViewSource;
            
            topBar.Add(backButton); 
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
                deleteButton.WithSameBottom(backButton)
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

            var set = this.CreateBindingSet<ContactDetailsViewController, ContactDetailsViewModel>();
            groupedTableViewSource.SelectedItemChanged += (e, a) =>
            {
                ViewModel.ChildItemTapped.Execute(groupedTableViewSource.SelectedItem);
            };
            
            set.Bind(backButton)
                .To(x => x.Close);

            set.Bind(deleteButton)
                .To(x => x.Delete);
            
            set.Bind(groupedTableViewSource)
                .For(x => x.ItemsSource)
                .To(x => x.Sections)
                .WithConversion(new GenericValueConverter<ObservableCollection<DetailsSection>, ObservableCollection<DetailsSection>>(sections =>
                {
               

                 var debtsStatistics = sections.FirstOrDefault(x => x is ContactDetailsFooterDebtsStatisticsSection);
                 if (debtsStatistics != null)
                     sections.Remove(debtsStatistics);
                 var loansStatistics = sections.FirstOrDefault(x => x is ContactDetailsFooterLoansStatisticsSection);
                 if (loansStatistics != null)
                     sections.Remove(loansStatistics);
             
                 var contactHeader = sections.FirstOrDefault(x => x is ContactDetailsHeaderSection);
                 if (contactHeader != null)
                     sections.Remove(contactHeader);
                 
                 sections.Insert(0, new ContactDetailsHeaderSection() 
                 { 
                     Title = TextResources.FinanceDetailsViewModel_General,
                     DataContext = ViewModel
                 });
                 
                 sections.Insert(1, new ContactDetailsFooterDebtsStatisticsSection()
                 {
                     Title = TextResources.ContactDetailsViewModel_StatsDebts,
                     DataContext = ViewModel
                 });
             
                sections.Insert(2, new ContactDetailsFooterLoansStatisticsSection()
                {
                    Title = TextResources.ContactDetailsViewModel_StatsLoans,
                    DataContext = ViewModel
                });
            
                return sections;
            }));
            
            set.Apply();
            SetupAppBar();
        }

        void SetupAppBar()
        {
            var mainViewController = UIApplication.SharedApplication.KeyWindow.RootViewController as MainViewController;
            var appBar = mainViewController.BottomAppBarView;
            
            appBar.SetFloatingButtonHidden(true, true);
            
            var callImage = UIImage.FromBundle("phone").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            var smsImage = UIImage.FromBundle("sms").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate); 
            
            var callMenuItem = new UIBarButtonItem(callImage,
                UIBarButtonItemStyle.Plain,
                (e, a) =>
                {
                    ViewModel.Call.Execute();
                });

            var smsMenuItem = new UIBarButtonItem(smsImage,
                UIBarButtonItemStyle.Plain,
                (e, a) =>
                {
                    ViewModel.Sms.Execute();
                });
                 

            List<UIBarButtonItem> trailingMenuItems = new List<UIBarButtonItem>()
            { 
                callMenuItem,
                smsMenuItem
            };
         
            var menuItem = new UIBarButtonItem(UIImage.FromBundle("menu_hamburger"), UIBarButtonItemStyle.Plain, (e, a) =>
            {
                MenuViewController.Show();
            });
            
            appBar.LeadingBarButtonItems = new UIBarButtonItem[] { menuItem };
            appBar.TrailingBarButtonItems = ViewModel.ArePhoneRelatedFeaturesEnabled ? trailingMenuItems.ToArray() : new UIBarButtonItem[] {};
        }
        
    }
}