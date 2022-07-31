using Debts.Model.Sections;
using MvvmCross.AdvancedRecyclerView.Data;
using MvvmCross.AdvancedRecyclerView.TemplateSelectors;
using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;

namespace Debts.Droid.Core.TemplateSelectors
{
    public class FinanceDetailsTemplateSelector : IMvxTemplateSelector, IMvxHeaderTemplate, IMvxFooterTemplate
    {
        public int HeaderLayoutId { get; set; }
        public int FooterLayoutId { get; set; }
        
        public FinanceDetailsTemplateSelector()
        {
        }

        public int GetItemViewType(object forItemObject)
        {
            if (forItemObject is MvxGroupedData groupedData)
            { 

                return Resource.Layout.finance_details_notes_section_header;
            }

            return GetChildItemViewType(forItemObject);
        }

        public int GetItemLayoutId(int fromViewType)
        {
            return fromViewType;
        }

        public int ItemTemplateId { get; set; }

        protected int GetChildItemViewType(object forItemObject) => Resource.Layout.finance_details_notes_section_item;
    }
}