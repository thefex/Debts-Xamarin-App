using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Debts.Converters.AppDomain;
using Debts.Data;
using Debts.iOS.Cells;
using Debts.iOS.Config;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Utilities;
using Debts.iOS.Utilities.TableView.Grouped;
using Debts.iOS.ViewControllers.Base;
using Debts.iOS.ViewControllers.Contacts;
using Debts.Model.Sections;
using Debts.Resources;
using Debts.ViewModel;
using Debts.ViewModel.Statistics;
using Foundation;
using MaterialComponents;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using UIKit;

namespace Debts.iOS.ViewControllers
{
    [MvxPagePresentation]
    public class StatisticsViewController : BaseViewController<StatisticsViewModel, string>
    {
        public StatisticsViewController()
        {
        }

        public StatisticsViewController(IntPtr handle) : base(handle)
        {
        }

        protected StatisticsViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
        } 
        public override async void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = AppColors.GrayBackground; 
            
            UITableView tableView = new UITableView(new CGRect(), UITableViewStyle.InsetGrouped)
            {
                SeparatorColor = AppColors.GrayBackground,
                AllowsSelection = true,
                AllowsMultipleSelection = false,
                BackgroundColor = AppColors.GrayBackground,
                SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine,
                ContentInset = new UIEdgeInsets(0, 0, 82, 0)
            };
            tableView.TableHeaderView = GetHeaderView();
            var groupedTableViewSource =  new MvxGroupedBaseTableViewSource(tableView, this);
            groupedTableViewSource.ParentViewControllerProvider = () => this;
            groupedTableViewSource.TemplateSelector = new StatisticsSectionTemplateSelector(tableView);
            groupedTableViewSource.ReloadOnAllItemsSourceSets = true;
            tableView.Source = groupedTableViewSource;
              
            Add(tableView); 
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
             
            View.AddConstraints(
                tableView.AtTopOfSafeArea(View),
                tableView.AtLeftOf(View),
                tableView.AtRightOf(View),
                tableView.AtBottomOf(View)
            );

            var set = this.CreateBindingSet<StatisticsViewController, StatisticsViewModel>();

            groupedTableViewSource.SelectedItemChanged += (e, a) =>
            {
                ViewModel.ChildItemTapped.Execute(groupedTableViewSource.SelectedItem);
            };
            
            set.Bind(groupedTableViewSource)
                .For(x => x.ItemsSource)
                .To(x => x.Sections)
                .WithConversion(new GenericValueConverter<ObservableCollection<DetailsSection>, ObservableCollection<DetailsSection>>(sections =>
                {
                    var budgetStatistics = sections.FirstOrDefault(x => x is StatisticsForBudgetSection);
                    if (budgetStatistics != null)
                        sections.Remove(budgetStatistics);
                    
                     var debtsStatistics = sections.FirstOrDefault(x => x is StatisticsForDebtsSection);
                     if (debtsStatistics != null)
                         sections.Remove(debtsStatistics);
                     var loansStatistics = sections.FirstOrDefault(x => x is StatisticsForLoansSection);
                     if (loansStatistics != null)
                         sections.Remove(loansStatistics);
                                  
                     sections.Insert(0, new StatisticsForBudgetSection()
                     {
                         Title = TextResources.StatisticsViewModel_BudgetText,
                         DataContext = ViewModel
                     });
                                  
                     sections.Insert(1, new StatisticsForDebtsSection() 
                     { 
                         Title = TextResources.StatisticsViewModel_DebtsText,
                         DataContext = ViewModel
                     });
                     
                     sections.Insert(2, new StatisticsForLoansSection()
                     {
                         Title = TextResources.StatisticsViewModel_LoanText,
                         DataContext = ViewModel
                     });
                
                    return sections; 
                }));
            
            set.Apply(); 
            
            (ViewModel as INotifyPropertyChanged).PropertyChanged += (e, a) =>
            {
                if (a.PropertyName == nameof(ViewModel.HasDateFilter) || a.PropertyName == nameof(ViewModel.FilterDates))
                    tableView.TableHeaderView = GetHeaderView();    
            };
        }
        
        UIView GetHeaderView()
        {
            UIView headerView = new UIView()
            {
                BackgroundColor = AppColors.GrayBackground
            };

            UILabel titleLabel = new UILabel
            {
                Font = UIFont.SystemFontOfSize(20, UIFontWeight.Regular),
                Text = TextResources.StatisticsViewModel_StatisticsText,
                TextAlignment = UITextAlignment.Left,
                TextColor = AppColors.GrayForTextFieldContainer
            };
            
            headerView.Add(titleLabel);

            headerView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            headerView.AddConstraints(
                titleLabel.AtTopOf(headerView, 12),
                titleLabel.AtLeftOf(headerView, 12),
                titleLabel.AtRightOf(headerView, 12)
            );
            
            if (!ViewModel.HasDateFilter)
                headerView.AddConstraints(titleLabel.AtBottomOf(headerView, 6));
            
            var dateConverter = new FinancesViewModelToDateRangeValueConverter();
            
            UILabel forDateLabel = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular),
                Text = dateConverter.Convert(ViewModel.FilterDates, null, null, null) as string,
                TextAlignment = UITextAlignment.Left,
                TextColor = AppColors.GrayForTextFieldContainer
            };
            
            UIButton closeImageView = new UIButton(UIButtonType.Close);  
            int heightConstant = 0;
            if (ViewModel.HasDateFilter)
            {
                headerView.Add(forDateLabel);
                headerView.Add(closeImageView);
                
                headerView.AddConstraints(
                    forDateLabel.Below(titleLabel, 0),
                    forDateLabel.AtLeftOf(headerView, 12),
                    forDateLabel.ToLeftOf(closeImageView),
                        
                    closeImageView.WithSameCenterY(forDateLabel),
                    closeImageView.Width().GreaterThanOrEqualTo(24),
                    closeImageView.AtRightOf(headerView, 12)
                );

                closeImageView.TouchUpInside += (e, a) => { ViewModel.ResetDateFilter.Execute(); };

                headerView.BringSubviewToFront(closeImageView);
                heightConstant = 48;
            }
			
            headerView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
			
            headerView.SetNeedsDisplay();
            headerView.LayoutIfNeeded();
            var height = headerView.SystemLayoutSizeFittingSize(UIView.UILayoutFittingCompressedSize).Height;
 
            var headerFrame = headerView.Frame;
            headerFrame.Width = View.Frame.Width;
            headerFrame.Height = height + heightConstant;
            headerView.Frame = headerFrame;
   
            return headerView;
        }
    }
}