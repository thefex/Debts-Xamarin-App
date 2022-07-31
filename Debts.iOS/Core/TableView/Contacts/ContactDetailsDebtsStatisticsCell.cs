using System;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.TableView.Sections;
using Debts.iOS.Core.ValueConverters;
using Debts.iOS.Utilities;
using Debts.Resources;
using Debts.ViewModel;
using MvvmCross.Binding.BindingContext;

namespace Debts.iOS.Core.TableView.Contacts
{
    public class ContactDetailsDebtsStatisticsCell : StatisticsCell
    {
        public ContactDetailsDebtsStatisticsCell(IntPtr handle) : base(handle)
        {
        }

        protected override void SetupBindings()
        {
            this.DelayBind(() =>
            {
                mainTitle.Text  = TextResources.ContactDetailsViewModel_StatsDebts;
                amountContent.Text = TextResources.ContactDetailsViewModel_AmountOfDebts;
                totalContent.Text = TextResources.ContactDetailsViewModel_TotalAmountDebts;
                remainingContent.Text = TextResources.ContactDetailsViewModel_RemainingDebts;
                collectedContent.Text = TextResources.ContactDetailsViewModel_CollectedDebts;
            
                var set = this
                    .CreateBindingSet<ContactDetailsDebtsStatisticsCell, ContactDetailsFooterDebtsStatisticsSection>();

                set.Bind(amountTitle)
                    .To(x => x.DataContext.AmountOfDebts);
                
                set.Bind(totalTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<ContactDetailsViewModel, string>(x => $"${x.TotalMoneyAmountOfDebts} ({x.TotalDebtsCurrency})"));
                
                set.Bind(remainingTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<ContactDetailsViewModel, string>(x => $"${x.RemainingMoneyAmountOfDebts} ({x.RemainingDebtsCurrency})"));
                
                set.Bind(collectedTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<ContactDetailsViewModel, string>(x => $"${x.CollectedMoneyAmountOfDebts} ({x.CollectedDebtsCurrency})"));
                
                set.Apply();
            });
        }
    }
}