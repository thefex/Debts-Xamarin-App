using MvvmCross.AdvancedRecyclerView.TemplateSelectors;
using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;

namespace Debts.Droid.Core.TemplateSelectors
{
    public class PickBudgetCategoryTemplateSelector : MvxDefaultTemplateSelector, IMvxHeaderTemplate, IMvxFooterTemplate
    { 
        public PickBudgetCategoryTemplateSelector() : base(Resource.Layout.budget_category_list_item)
        {
        }

        public int HeaderLayoutId { get; set; }
          
        public int FooterLayoutId { get; set; }
    }
}