using System;
using System.Collections.Generic;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Debts.Data;
using Debts.iOS.Config;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Utilities;
using Debts.iOS.Utilities.TableView.Grouped;
using Debts.iOS.ViewControllers.Base;
using Debts.Model.Sections;
using Debts.Resources;
using Debts.ViewModel.Budget;
using FFImageLoading.Extensions;
using FFImageLoading.Work;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers.Budget
{
    [MvxPagePresentation]
    public class BudgetDetailsViewController : BaseViewController<BudgetDetailsViewModel, BudgetItem>
    { 
        public BudgetDetailsViewController()
        {
        }

        public BudgetDetailsViewController(IntPtr handle) : base(handle)
        {
        }

        protected BudgetDetailsViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
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
            var groupedTableViewSource = new MvxGroupedBaseTableViewSource(tableView, this);
            groupedTableViewSource.ParentViewControllerProvider = () => this;
            groupedTableViewSource.TemplateSelector = new BudgetDetailsSectionTemplateSelector(tableView);
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

            var set = this.CreateBindingSet<BudgetDetailsViewController, BudgetDetailsViewModel>();

            set.Bind(backButton)
                .To(x => x.Close);

            set.Bind(deleteButton)
                .To(x => x.Delete);
            
            set.Bind(groupedTableViewSource)
                .For(x => x.ItemsSource)
                .To(x => x.Sections)
                .WithConversion(new GenericValueConverter<List<DetailsSection>, List<DetailsSection>>(sections =>
                {
                    var convertedSections = new List<DetailsSection>(sections);
                    convertedSections.Insert(0, new BudgetDetailsHeaderSection() 
                    { 
                        Title = TextResources.FinanceDetailsViewModel_General,
                        DataContext = ViewModel
                    });

                    return convertedSections;
                }));
            
            set.Apply(); 
        } 
    }
}