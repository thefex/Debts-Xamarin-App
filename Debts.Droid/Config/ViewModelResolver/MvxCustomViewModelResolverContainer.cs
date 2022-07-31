using System;
using System.Collections.Generic;
using Debts.ViewModel.Budget;
using Debts.ViewModel.Finances;
using Debts.ViewModel.Statistics;
using MvvmCross.ViewModels;

namespace Debts.Droid.Config.ViewModelResolver
{
    internal class MvxCustomViewModelResolverContainer
    {
        private readonly Dictionary<Type, MvxCustomViewModelResolver> _resolverContainer = new Dictionary
            <Type, MvxCustomViewModelResolver>
            {
                {typeof(FilterFinancesByDateViewModel), new FilterFinancesByDateViewModelResolver()},
                {typeof(FilterFinancesByStateViewModel), new FilterFinancesByStateViewModelResolver()},
                {typeof(FilterStatisticsByDateViewModel), new FilterStatisticsByDateViewModelResolver()},
                {typeof(FilterBudgetByDateViewModel), new FilterBudgetByDateViewModelResolver()},
                {typeof(FilterBudgetByCategoryViewModel), new FilterBudgetByCategoryViewModelResolver()}
            };

        public bool HasRegisteredCustomViewModelResolver(Type forType) => _resolverContainer.ContainsKey(forType);

        public IMvxViewModel ResolveViewModel(Type forType) => _resolverContainer[forType].GetViewModel();
    }
}