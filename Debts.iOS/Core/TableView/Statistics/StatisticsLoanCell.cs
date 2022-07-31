using System;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.TableView.Sections;
using Debts.iOS.Core.ValueConverters;
using Debts.Resources;
using Debts.ViewModel;
using Debts.ViewModel.Statistics;
using MvvmCross.Binding.BindingContext;

namespace Debts.iOS.Core.TableView.Statistics
{
    public class StatisticsLoanCell : StatisticsCell
    {
        public StatisticsLoanCell(IntPtr ptr) : base(ptr)
        {
        }

        protected override void SetupBindings()
        {
            this.DelayBind(() =>
            {
                mainTitle.Text  = TextResources.StatisticsViewModel_LoanText;
                amountContent.Text = TextResources.StatisticsViewModel_AmountOfLoans;
                totalContent.Text = TextResources.StatisticsViewModel_TotalAmountOfLoanMoney;
                remainingContent.Text = TextResources.StatisticsViewModel_RemainingAmountOfLoanMoney;
                collectedContent.Text = TextResources.StatisticsViewModel_PaidAmountOfLoanMoney;
            
                var set = this
                    .CreateBindingSet<StatisticsLoanCell, StatisticsForLoansSection>();

                set.Bind(amountTitle)
                    .To(x => x.DataContext.AmountOfLoans);
                
                set.Bind(totalTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<StatisticsViewModel, string>(x => $"${x.TotalMoneyAmountOfLoans} ({x.TotalLoansCurrency})"));
                
                set.Bind(remainingTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<StatisticsViewModel, string>(x => $"${x.RemainingMoneyAmountOfLoans} ({x.RemainingLoansCurrency})"));
                
                set.Bind(collectedTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<StatisticsViewModel, string>(x => $"${x.CollectedMoneyAmountOfLoans} ({x.CollectedLoansCurrency})"));
                
                set.Apply();
            });
        }
    }
}