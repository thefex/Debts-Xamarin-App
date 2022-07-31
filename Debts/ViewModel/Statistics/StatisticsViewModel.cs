using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Debts.Commands;
using Debts.Commands.FinanceDetails;
using Debts.Data;
using Debts.Data.Queries;
using Debts.DataAccessLayer;
using Debts.DataAccessLayer.Budget;
using Debts.DataAccessLayer.FinanceOperations;
using Debts.Model.Sections;
using Debts.Resources;
using Debts.Services;
using Debts.Services.AppGrowth;
using Debts.Services.Settings;
using Debts.ViewModel.AppGrowth;
using DynamicData;
using MvvmCross.Commands;

namespace Debts.ViewModel.Statistics
{
    public class StatisticsViewModel : BaseViewModel<string>
    {
        private readonly QueryCommandExecutor _queryCommandExecutor;
        private readonly SettingsService _settingsService;
        private readonly PremiumService _premiumService;

        public StatisticsViewModel(QueryCommandExecutor queryCommandExecutor, SettingsService settingsService, PremiumService premiumService)
        {
            _queryCommandExecutor = queryCommandExecutor;
            _settingsService = settingsService;
            _premiumService = premiumService;
        }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
        
        public DateTime?[] FilterDates => new[] {StartDate, EndDate};

        public bool HasDateFilter => StartDate.HasValue && EndDate.HasValue;
        
        public override async void Prepare()
        {
            base.Prepare();
            
            var sections = new ObservableCollection<DetailsSection>();
            Sections = sections;
            
            var operations = await _queryCommandExecutor.Execute(new GetAllFinanceOperationsQuery(new FinanceOperationsQueryParameter()
            {
                Limit = 10*1000,
                Offset = 0,
                IsActivePaymentEnabled = true,
                IsPaidOffPaymentEnabled = true,
                IsPaymentDeadlineExceedEnabled = true,
                StartDate = StartDate,
                EndDate = EndDate
            }));
            
            var budgets = await _queryCommandExecutor.Execute(new BudgetGetAllQuery(new BudgetItemsQueryParameter()
            {
                Limit = 10 * 1000,
                Offset = 0,
                StartDate = StartDate,
                EndDate = EndDate
            }));

            
            sections.Add(
                new DetailsFinanceOperationsSection() {
                        Title = TextResources.Transactions,
                        Operations = new ObservableCollection<FinanceOperation>(operations)
                    }
                );
                
            CalculateStatistics(operations.ToList(), budgets.ToList());
            RaisePropertyChanged(nameof(Sections));
        }

        private async Task ReloadData()
        {
            var operations = await _queryCommandExecutor.Execute(new GetAllFinanceOperationsQuery(new FinanceOperationsQueryParameter()
            {
                Limit = 10*1000,
                Offset = 0,
                IsActivePaymentEnabled = true,
                IsPaidOffPaymentEnabled = true,
                IsPaymentDeadlineExceedEnabled = true,
                StartDate = StartDate,
                EndDate = EndDate
            }));

            var budgets = await _queryCommandExecutor.Execute(new BudgetGetAllQuery(new BudgetItemsQueryParameter()
            {
                Limit = 10 * 1000,
                Offset = 0,
                StartDate = StartDate,
                EndDate = EndDate
            }));

            var detailsFinanceOperationsSection = Sections.FirstOrDefault() as DetailsFinanceOperationsSection;

            if (detailsFinanceOperationsSection != null)
            {
                detailsFinanceOperationsSection.Operations.Clear();
                detailsFinanceOperationsSection.Operations.AddRange(operations);
                CalculateStatistics(operations.ToList(), budgets.ToList());
            }
        }
        
        private void CalculateStatistics(IList<FinanceOperation> operations, IList<BudgetItem> budgets)
        {
            AmountOfDebts = operations.Count(x => x.IsDebt);
            var totalDebts = operations.Where(x => x.IsDebt);
            TotalMoneyAmountOfDebts = totalDebts.Sum(x => x.PaymentDetails.Amount);
            var multipleCurrenciesText = TextResources.MultipleCurrencies;
            
            TotalDebtsCurrency = totalDebts.Select(x => x.PaymentDetails.Currency).Distinct().Count() > 1
                ? multipleCurrenciesText
                : totalDebts.FirstOrDefault()?.PaymentDetails?.Currency ?? _settingsService.GetDefaultCurrency();

            var remainingDebts = totalDebts.Where(x => !x.IsPaid);
            RemainingMoneyAmountOfDebts = remainingDebts.Sum(x => x.PaymentDetails.Amount);
            RemainingDebtsCurrency = remainingDebts.Select(x => x.PaymentDetails.Currency).Distinct().Count() > 1
                ? multipleCurrenciesText
                : remainingDebts.FirstOrDefault()?.PaymentDetails?.Currency ?? _settingsService.GetDefaultCurrency();

            var collectedDebts = totalDebts.Where(x => x.IsPaid);
            CollectedMoneyAmountOfDebts = collectedDebts.Sum(x => x.PaymentDetails.Amount);
            CollectedDebtsCurrency = collectedDebts.Select(x => x.PaymentDetails.Currency).Distinct().Count() > 1
                ? multipleCurrenciesText
                : collectedDebts.FirstOrDefault()?.PaymentDetails?.Currency ?? _settingsService.GetDefaultCurrency();
                
            AmountOfLoans = operations.Count(x => x.IsLoan);

            var totalLoans = operations.Where(x => x.IsLoan);
            TotalMoneyAmountOfLoans = totalLoans.Sum(x => x.PaymentDetails.Amount);
            TotalLoansCurrency = totalLoans.Select(x => x.PaymentDetails.Currency).Distinct().Count() > 1
                ? multipleCurrenciesText
                : totalLoans.FirstOrDefault()?.PaymentDetails?.Currency ?? _settingsService.GetDefaultCurrency();

            var remainingLoans = totalLoans.Where(x => !x.IsPaid);
            RemainingMoneyAmountOfLoans = remainingLoans.Sum(x => x.PaymentDetails.Amount);
            RemainingLoansCurrency = remainingLoans.Select(x => x.PaymentDetails.Currency).Distinct().Count() > 1
                ? multipleCurrenciesText
                : remainingLoans.FirstOrDefault()?.PaymentDetails?.Currency ?? _settingsService.GetDefaultCurrency();

            var collectedLoans = totalLoans.Where(x => x.IsPaid);
            CollectedMoneyAmountOfLoans = collectedLoans.Sum(x => x.PaymentDetails.Amount);
            CollectedLoansCurrency = collectedLoans.Select(x => x.PaymentDetails.Currency).Distinct().Count() > 1
                ? multipleCurrenciesText
                : collectedLoans.FirstOrDefault()?.PaymentDetails?.Currency ?? _settingsService.GetDefaultCurrency();

            AmountOfBudgetItems = budgets.Count();
            var incomeBudgetItems = budgets.Where(x => x.Type == BudgetType.Income);
            IncomeAmount = incomeBudgetItems.Sum(x => x.Amount);
            
            var expensesBudgetItems = budgets.Where(x => x.Type == BudgetType.Expense);
            ExpensesAmount = expensesBudgetItems.Sum(x => x.Amount);
            TotalBudgetAmount = budgets.Sum(x => x.Amount);
            
            IncomeCurrency =  incomeBudgetItems.Select(x => x.Currency).Distinct().Count() > 1
                ? multipleCurrenciesText
                : incomeBudgetItems.FirstOrDefault()?.Currency ?? _settingsService.GetDefaultCurrency();
            
            ExpensesCurrency = expensesBudgetItems.Select(x => x.Currency).Distinct().Count() > 1
                ? multipleCurrenciesText
                : expensesBudgetItems.FirstOrDefault()?.Currency ?? _settingsService.GetDefaultCurrency();
            
            TotalBudgetCurrency = budgets.Select(x => x.Currency).Distinct().Count() > 1 
                                  ? multipleCurrenciesText
                                  : budgets.FirstOrDefault()?.Currency ?? _settingsService.GetDefaultCurrency();
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
        
        public decimal AmountOfBudgetItems { get; private set; }
        
        public string ExpensesCurrency { get; private set; }
        
        public string TotalBudgetCurrency { get; private set; }
        
        public string IncomeCurrency { get; private set; }
        
        public decimal ExpensesAmount { get; private set; }
        
        public decimal IncomeAmount { get; private set; }
        
        public decimal TotalBudgetAmount { get; private set; }

        public IEnumerable<DetailsSection> Sections { get; private set; } =
            Enumerable.Empty<DetailsSection>();
        
        public MvxCommand ResetDateFilter => new MvxExceptionGuardedCommand(() =>
        {
            StartDate = null;
            EndDate = null;
            Filter.Execute();
        });
        
        public MvxCommand FilterByDate => new MvxExceptionGuardedCommand(() =>
        {
            if (!_premiumService.HasPremium)
            {
                ServicesLocation.NavigationService.Navigate<GoPremiumViewModel>();
                return;
            }
            
            ServicesLocation.NavigationService.Navigate<FilterStatisticsByDateViewModel>();
        });

        
        public MvxCommand Filter => new MvxExceptionGuardedCommand(async () =>
        {
            await ReloadData();
            RaisePropertyChanged(() => HasDateFilter);
            RaisePropertyChanged(() => FilterDates);
        });
        
        public MvxCommand<object> ChildItemTapped => new MvxExceptionGuardedCommand<object>(item =>
        {
            if (item is FinanceOperation)
                new TransferToFinanceDetailsCommandBuilder(_queryCommandExecutor).BuildCommand().Execute(item);
        });
    }
}