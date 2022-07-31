using System;
using System.Collections.Generic;
using Debts.ViewModel.Budget;
using Debts.ViewModel.Finances;
using Debts.ViewModel.Statistics;
using MvvmCross.ViewModels;

namespace Debts.iOS.Config.ViewModelResolver
{
    internal class MvxCustomViewModelResolverContainer
    {
        private readonly Dictionary<Type, IMvxCustomViewModelResolver> _resolverContainer = new Dictionary
            <Type, IMvxCustomViewModelResolver>
            {
                {typeof(FilterFinancesByDateViewModel), new FilterFinancesByDateViewModelResolver()},
                {typeof(FilterFinancesByStateViewModel), new FilterFinancesByStateViewModelResolver()},
                {typeof(FilterStatisticsByDateViewModel), new FilterStatisticsByDateViewModelResolver()},
                {typeof(FilterBudgetByDateViewModel), new FilterBudgetDateViewModelResolver()},
                {typeof(FilterBudgetByCategoryViewModel), new FilterBudgetCategoryViewModelResolver()}
            };

        public bool HasRegisteredCustomViewModelResolver(Type forType) => _resolverContainer.ContainsKey(forType);

        public IMvxViewModel ResolveViewModel(Type forType) => _resolverContainer[forType].GetViewModel();
    }
}