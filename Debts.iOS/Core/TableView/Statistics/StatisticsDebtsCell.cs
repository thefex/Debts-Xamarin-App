using System;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.TableView.Contacts;
using Debts.iOS.Core.TableView.Sections;
using Debts.iOS.Core.ValueConverters;
using Debts.Resources;
using Debts.ViewModel;
using Debts.ViewModel.Statistics;
using MvvmCross.Binding.BindingContext;

namespace Debts.iOS.Core.TableView.Statistics
{
    public class StatisticsDebtsCell : StatisticsCell
    {
        public StatisticsDebtsCell(IntPtr handle) : base(handle)
        {
        }

        protected override void SetupBindings()
        {
            this.DelayBind(() =>
            {
                mainTitle.Text  = TextResources.StatisticsViewModel_DebtsText;
                amountContent.Text = TextResources.StatisticsViewModel_AmountOfDebts;
                totalContent.Text = TextResources.StatisticsViewModel_TotalAmountOfDebtsMoney;
                remainingContent.Text = TextResources.StatisticsViewModel_RemainingAmountOfDebtMoney;
                collectedContent.Text = TextResources.StatisticsViewModel_CollectedAmountOfDebtMoney;
        
                var set = this
                    .CreateBindingSet<StatisticsDebtsCell, StatisticsForDebtsSection>();

                set.Bind(amountTitle)
                    .To(x => x.DataContext.AmountOfDebts);
                
                set.Bind(totalTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<StatisticsViewModel, string>(x => $"${x.TotalMoneyAmountOfDebts} ({x.TotalDebtsCurrency})"));
                
                set.Bind(remainingTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<StatisticsViewModel, string>(x => $"${x.RemainingMoneyAmountOfDebts} ({x.RemainingDebtsCurrency})"));
                
                set.Bind(collectedTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<StatisticsViewModel, string>(x => $"${x.CollectedMoneyAmountOfDebts} ({x.CollectedDebtsCurrency})"));

                set.Apply();
            });
        }
    }
}