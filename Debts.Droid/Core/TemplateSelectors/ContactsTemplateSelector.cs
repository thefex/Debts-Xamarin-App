using Debts.Data;
using MvvmCross.AdvancedRecyclerView.TemplateSelectors;

namespace Debts.Droid.Core.TemplateSelectors
{
    public class ContactsTemplateSelector : MvxExpandableTemplateSelector, IMvxHeaderTemplate, IMvxFooterTemplate
    { 
        public ContactsTemplateSelector() : base(Resource.Layout.contact_group_letter_item_template)
        {
        }

        public int HeaderLayoutId { get; set; }
        
        protected override int GetChildItemViewType(object forItemObject)
        {
            return Resource.Layout.contact_item_template;
        }

        protected override int GetChildItemLayoutId(int fromViewType)
        {
            return fromViewType;
        }

        public int FooterLayoutId { get; set; }
    }
}