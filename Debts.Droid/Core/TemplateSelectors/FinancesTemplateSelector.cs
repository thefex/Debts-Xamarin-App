using System;
using Debts.Data;
using MvvmCross.AdvancedRecyclerView.TemplateSelectors;

namespace Debts.Droid.Core.TemplateSelectors
{
    public class FinancesTemplateSelector : MvxExpandableTemplateSelector, IMvxHeaderTemplate, IMvxFooterTemplate
    { 
        public FinancesTemplateSelector() : base(Resource.Layout.finances_list_group_header)
        {
        }

        protected override int GetChildItemViewType(object forItemObject)
        {
            if (forItemObject is FinanceOperation operation && operation.IsDebt)
                return Resource.Layout.debt_finance_item;
            
            return Resource.Layout.loan_finance_item;
        }

        protected override int GetChildItemLayoutId(int fromViewType)
        {
            return fromViewType;
        }
        
        public int HeaderLayoutId { get; set; }

        public int FooterLayoutId { get; set; }
    }
}