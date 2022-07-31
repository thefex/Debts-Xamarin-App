using System;
using Cirrious.FluentLayouts.Touch;
using Debts.iOS.Config;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.TableView.Sections;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Utilities;
using Debts.Resources;
using Debts.ViewModel.Statistics;
using MvvmCross.Binding.BindingContext;
using UIKit;

namespace Debts.iOS.Core.TableView.Statistics
{
    public class StatisticsBudgetCell : StatisticsCell
    {  
        protected StatisticsBudgetCell(IntPtr ptr) : base(ptr)
        { 
        }
 
        protected override void SetupBindings()
        {
            this.DelayBind(() =>
            {
                mainTitle.Text  = TextResources.StatisticsViewModel_BudgetText;
                amountContent.Text = TextResources.StatisticsViewModel_AmountOfBudget;
                totalContent.Text = TextResources.StatisticsViewModel_TotalAmountOfBudget;
                remainingContent.Text = TextResources.StatisticsViewModel_Expenses;
                collectedContent.Text = TextResources.StatisticsViewModel_Income;
        
                var set = this
                    .CreateBindingSet<StatisticsBudgetCell, StatisticsForBudgetSection>();

                set.Bind(amountTitle)
                    .To(x => x.DataContext.AmountOfBudgetItems);
                
                set.Bind(totalTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<StatisticsViewModel, string>(x => $"${x.TotalBudgetAmount} ({x.TotalBudgetCurrency})"));
                
                set.Bind(remainingTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<StatisticsViewModel, string>(x => $"${x.ExpensesAmount} ({x.ExpensesCurrency})"));
                
                set.Bind(collectedTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<StatisticsViewModel, string>(x => $"${x.IncomeAmount} ({x.IncomeCurrency})"));

                set.Apply();
            });
        }
    }
}