using MvvmCross.AdvancedRecyclerView.TemplateSelectors;
using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;

namespace Debts.Droid.Core.TemplateSelectors
{
    public class BudgetDetailsTemplateSelector : MvxDefaultTemplateSelector, IMvxHeaderTemplate, IMvxFooterTemplate
    {
        public BudgetDetailsTemplateSelector() : base(Resource.Layout.blank_footer)
        {
        }

        public int HeaderLayoutId { get; set; }
        public int FooterLayoutId { get; set; }
    }
}