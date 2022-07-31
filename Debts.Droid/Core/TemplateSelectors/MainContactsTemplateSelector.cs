using Debts.Data;
using MvvmCross.AdvancedRecyclerView.TemplateSelectors;
using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;

namespace Debts.Droid.Core.TemplateSelectors
{
    public class MainContactsTemplateSelector : MvxExpandableTemplateSelector, IMvxHeaderTemplate, IMvxFooterTemplate
    { 
        public MainContactsTemplateSelector() : base(Resource.Layout.contact_group_letter_item_template)
        {
        }

        public int HeaderLayoutId { get; set; }
        
        protected override int GetChildItemViewType(object forItemObject)
        {
            return Resource.Layout.contact_item_template_with_active_operations_count;
        }

        protected override int GetChildItemLayoutId(int fromViewType)
        {
            return fromViewType;
        }

        public int FooterLayoutId { get; set; }
    }
}