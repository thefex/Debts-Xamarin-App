using System;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.TableView.Sections;
using Debts.iOS.Core.ValueConverters;
using Debts.Resources;
using Debts.ViewModel;
using MvvmCross.Binding.BindingContext;

namespace Debts.iOS.Core.TableView.Contacts
{
    public class ContactDetailsLoanStatisticsCell : StatisticsCell
    {
        public ContactDetailsLoanStatisticsCell(IntPtr handle) : base(handle)
        {
        }

        protected override void SetupBindings()
        {
            this.DelayBind(() =>
            {
                mainTitle.Text  = TextResources.ContactDetailsViewModel_StatsLoans;
                amountContent.Text = TextResources.ContactDetailsViewModel_AmountOfLoans;
                totalContent.Text = TextResources.ContactDetailsViewModel_TotalAmountOfLoans;
                remainingContent.Text = TextResources.ContactDetailsViewModel_RemainingAmountLoan;
                collectedContent.Text = TextResources.ContactDetailsViewModel_PaidAmountLoan;
            
                var set = this
                    .CreateBindingSet<ContactDetailsLoanStatisticsCell, ContactDetailsFooterLoansStatisticsSection>();

                set.Bind(amountTitle)
                    .To(x => x.DataContext.AmountOfLoans);
                
                set.Bind(totalTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<ContactDetailsViewModel, string>(x => $"${x.TotalMoneyAmountOfLoans} ({x.TotalLoansCurrency})"));
                
                set.Bind(remainingTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<ContactDetailsViewModel, string>(x => $"${x.RemainingMoneyAmountOfLoans} ({x.RemainingLoansCurrency})"));
                
                set.Bind(collectedTitle)
                    .To(x => x.DataContext)
                    .WithConversion(new GenericValueConverter<ContactDetailsViewModel, string>(x => $"${x.CollectedMoneyAmountOfLoans} ({x.CollectedLoansCurrency})"));
                
                set.Apply();
            });
        }
    }
}