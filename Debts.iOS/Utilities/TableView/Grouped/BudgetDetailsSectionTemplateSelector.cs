using System;
using System.Collections;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.TableView.Sections;
using Debts.Model.Sections;
using MvvmCross.Binding.Extensions;
using UIKit;

namespace Debts.iOS.Utilities.TableView.Grouped
{
    public class BudgetDetailsSectionTemplateSelector : MvxGroupedTemplateSelector
    {
        public BudgetDetailsSectionTemplateSelector(UITableView tableView) : base(tableView)
        {
        }

        public override Type GetCellType(object forDataContext)
        {     
            if (forDataContext is BudgetDetailsHeaderSection)
                return typeof(BudgetDetailsHeaderCell);
                  
            throw new InvalidOperationException();
        }

        public override object GetItemAt(IEnumerable itemsSource, int groupIndex, int itemIndex)
        {
            var group = GetGroupItemAt(itemsSource, groupIndex) as DetailsSection; 
            return group;
        }

        public override object GetGroupItemAt(IEnumerable itemsSource, int groupIndex) => (itemsSource.ElementAt(groupIndex) as DetailsSection);

        public override int GetNumberOfItemsInGroup(IEnumerable itemsSource, int section)
        {
            var groupItem = GetGroupItemAt(itemsSource, section);
  
            if (groupItem is BudgetDetailsHeaderSection)
                return 1;

            return 0;
        }

        protected override Type GetGroupHeaderCellType(object fromDataContext) => null;
 

        protected override Type GetGroupFooterCellType(object fromDataContext) => null;
    }
}