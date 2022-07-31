using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Debts.Commands;
using Debts.Commands.ContactDetails;
using Debts.Commands.FinanceDetails;
using Debts.Data;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.Contacts;
using Debts.Model.Sections;
using Debts.Resources;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.Phone;
using Debts.Services.Settings;
using Humanizer;
using MvvmCross.Commands;

namespace Debts.ViewModel
{
    public class ContactDetailsViewModel : BaseViewModel<ContactDetails>
    {
        private readonly PhoneCallServices _phoneCallServices;
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly SettingsService _settingsService;
        private readonly PremiumService _premiumService;

        public ContactDetailsViewModel(PhoneCallServices phoneCallServices,
            QueryCommandExecutor queryCommandExecutor,
            SettingsService settingsService,
            PremiumService premiumService)
        {
            _phoneCallServices = phoneCallServices;
            _queryCommandExecutor = queryCommandExecutor;
            _settingsService = settingsService;
            _premiumService = premiumService;
        }
        
        public override async void Prepare(ContactDetails parameter)
        {
            base.Prepare(parameter);
            Details = parameter;
            Details = await _queryCommandExecutor.Execute(new GetContactDetailsQuery(Details));

            Sections.Add(new DetailsFinanceOperationsSection()
            {
                Title = TextResources.ActiveDebts,
                Operations = new ObservableCollection<FinanceOperation>(Details.Operations.Where(x => x.IsDebt && x.IsActive).ToList())  
            });
            Sections.Add(new DetailsFinanceOperationsSection()
            {
                Title = TextResources.ActiveLoans,
                Operations = new ObservableCollection<FinanceOperation>(Details.Operations.Where(x => x.IsLoan && x.IsActive).ToList())  
            });
            Sections.Add(new DetailsFinanceOperationsSection()
            {
                Title = TextResources.PastTransactions,
                Operations = new ObservableCollection<FinanceOperation>(Details.Operations.Where(x => x.IsPaid).ToList())  
            });
            RaisePropertyChanged(() => Sections);

            CalculateStatistics();
            _phoneCallServices.Disconnected += PhoneCallServicesOnDisconnected;
        }

        private void CalculateStatistics()
        {
            var operations = Details.Operations;
            AmountOfDebts = operations.Count(x => x.IsDebt);
            var totalDebts = operations.Where(x => x.IsDebt);
            TotalMoneyAmountOfDebts = totalDebts.Sum(x => x.PaymentDetails.Amount);
            TotalDebtsCurrency = totalDebts.Select(x => x.PaymentDetails.Currency).Distinct().Count() > 1
                ? TextResources.MultipleCurrencies
                : totalDebts.FirstOrDefault()?.PaymentDetails?.Currency ?? _settingsService.GetDefaultCurrency();

            var remainingDebts = totalDebts.Where(x => !x.IsPaid);
            RemainingMoneyAmountOfDebts = remainingDebts.Sum(x => x.PaymentDetails.Amount);
            RemainingDebtsCurrency = remainingDebts.Select(x => x.PaymentDetails.Currency).Distinct().Count() > 1
                ? TextResources.MultipleCurrencies
                : remainingDebts.FirstOrDefault()?.PaymentDetails?.Currency ?? _settingsService.GetDefaultCurrency();

            var collectedDebts = totalDebts.Where(x => x.IsPaid);
            CollectedMoneyAmountOfDebts = collectedDebts.Sum(x => x.PaymentDetails.Amount);
            CollectedDebtsCurrency = collectedDebts.Select(x => x.PaymentDetails.Currency).Distinct().Count() > 1
                ? TextResources.MultipleCurrencies
                : collectedDebts.FirstOrDefault()?.PaymentDetails?.Currency ?? _settingsService.GetDefaultCurrency();
                
            AmountOfLoans = operations.Count(x => x.IsLoan);

            var totalLoans = operations.Where(x => x.IsLoan);
            TotalMoneyAmountOfLoans = totalLoans.Sum(x => x.PaymentDetails.Amount);
            TotalLoansCurrency = totalLoans.Select(x => x.PaymentDetails.Currency).Distinct().Count() > 1
                ? TextResources.MultipleCurrencies
                : totalLoans.FirstOrDefault()?.PaymentDetails?.Currency ?? _settingsService.GetDefaultCurrency();

            var remainingLoans = totalLoans.Where(x => !x.IsPaid);
            RemainingMoneyAmountOfLoans = remainingLoans.Sum(x => x.PaymentDetails.Amount);
            RemainingLoansCurrency = remainingLoans.Select(x => x.PaymentDetails.Currency).Distinct().Count() > 1
                ? TextResources.MultipleCurrencies
                : remainingLoans.FirstOrDefault()?.PaymentDetails?.Currency ?? _settingsService.GetDefaultCurrency();

            var collectedLoans = totalLoans.Where(x => x.IsPaid);
            CollectedMoneyAmountOfLoans = collectedLoans.Sum(x => x.PaymentDetails.Amount);
            CollectedLoansCurrency = collectedLoans.Select(x => x.PaymentDetails.Currency).Distinct().Count() > 1
                ? TextResources.MultipleCurrencies
                : collectedLoans.FirstOrDefault()?.PaymentDetails?.Currency ?? _settingsService.GetDefaultCurrency();
        }

        public int AmountOfDebts { get; private set; }
        
        public decimal TotalMoneyAmountOfDebts { get; private set; }
        
        public string TotalDebtsCurrency { get; private set; }
        
        public decimal RemainingMoneyAmountOfDebts { get; private set; }
        
        public string RemainingDebtsCurrency { get; private set; }
        
        public decimal CollectedMoneyAmountOfDebts { get; private set; }
        
        public string CollectedDebtsCurrency { get; private set; }
        
        public int AmountOfLoans { get; private set; }
        public decimal TotalMoneyAmountOfLoans { get; private set; }
        
        public string TotalLoansCurrency { get; private set; }
        
        public decimal RemainingMoneyAmountOfLoans { get; private set; }
        
        public string RemainingLoansCurrency { get; private set; }
        
        public decimal CollectedMoneyAmountOfLoans { get; private set; }
        
        public string CollectedLoansCurrency { get; private set; }

        private void PhoneCallServicesOnDisconnected()
        {
            
        }

        public ContactDetails Details { get; private set; }
        
        public ObservableCollection<DetailsSection> Sections { get; } = new ObservableCollection<DetailsSection>();
        
        public bool ArePhoneRelatedFeaturesEnabled => !string.IsNullOrEmpty(Details.PhoneNumber);

        public MvxCommand Close => new MvxExceptionGuardedCommand(() => ServicesLocation.NavigationService.Close(this));
 
        public MvxCommand Delete => new DeleteContactAsyncGuardedCommandBuilder(_queryCommandExecutor, this).BuildCommand();
  
        public MvxCommand Call => new CallContactAsyncGuardedCommandBuilder(_phoneCallServices, Details, _premiumService).BuildCommand();
        
        public MvxCommand Sms => new SendMessageToContactAsyncGuardedCommandBuilder(Details).BuildCommand();

        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);
            _phoneCallServices.Disconnected -= PhoneCallServicesOnDisconnected;
        }
        
        public MvxCommand<object> ChildItemTapped => new MvxExceptionGuardedCommand<object>(item =>
        {
            if (item is FinanceOperation)
                new TransferToFinanceDetailsCommandBuilder(_queryCommandExecutor).BuildCommand().Execute(item);
        });
    }
}