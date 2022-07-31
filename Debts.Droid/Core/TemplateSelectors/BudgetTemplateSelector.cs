using System;
using Debts.Data;
using MvvmCross.AdvancedRecyclerView.TemplateSelectors;

namespace Debts.Droid.Core.TemplateSelectors
{
    public class BudgetTemplateSelector : MvxExpandableTemplateSelector, IMvxHeaderTemplate, IMvxFooterTemplate
    { 
        public BudgetTemplateSelector() : base(Resource.Layout.budget_list_group_header)
        {
        }

        protected override int GetChildItemViewType(object forItemObject)
        {
            if (forItemObject is BudgetItem budgetItem)
            {
                return budgetItem.IsExpense
                    ? Resource.Layout.budget_list_expense_item
                    : Resource.Layout.budget_list_income_item;
            }
            
            throw new InvalidOperationException(forItemObject?.ToString());
        }

        protected override int GetChildItemLayoutId(int fromViewType)
        {
            return fromViewType;
        }
        
        public int HeaderLayoutId { get; set; }

        public int FooterLayoutId { get; set; }
    }
}