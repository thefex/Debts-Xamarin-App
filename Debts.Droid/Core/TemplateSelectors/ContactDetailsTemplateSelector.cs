using System;
using Debts.Data;
using Debts.Model.Sections;
using MvvmCross.AdvancedRecyclerView.Data;
using MvvmCross.AdvancedRecyclerView.TemplateSelectors;
using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;

namespace Debts.Droid.Core.TemplateSelectors
{
    public class ContactDetailsTemplateSelector : IMvxTemplateSelector, IMvxHeaderTemplate, IMvxFooterTemplate
    {
        public int HeaderLayoutId { get; set; }
        public int FooterLayoutId { get; set; }
        
        public ContactDetailsTemplateSelector()
        {
        }

        public int GetItemViewType(object forItemObject)
        {
            if (forItemObject is MvxGroupedData groupedData)
            { 
                if (groupedData.Key is DetailsFinanceOperationsSection)
                    return Resource.Layout.statistics_section_finance_operation;
                
                throw new InvalidOperationException();
            }

            return GetChildItemViewType(forItemObject);
        }

        public int GetItemLayoutId(int fromViewType)
        {
            return fromViewType;
        }

        public int ItemTemplateId { get; set; }

        protected int GetChildItemViewType(object forItemObject)
        {
            if (forItemObject is FinanceOperation financeOperation && financeOperation.IsDebt)
                return Resource.Layout.transaction_debt_finance_item;

            return Resource.Layout.transaction_loan_finance_item;
        } 
    }
}