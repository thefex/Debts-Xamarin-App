using System;
using System.Collections;
using Debts.Data;
using Debts.iOS.Cells;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.TableView.Contacts;
using Debts.iOS.Core.TableView.Sections;
using Debts.Model.Sections;
using MvvmCross.Binding.Extensions;
using UIKit;

namespace Debts.iOS.Utilities.TableView.Grouped
{
    public class ContactDetailsSectionTemplateSelector : MvxGroupedTemplateSelector
    {
        public ContactDetailsSectionTemplateSelector(UITableView tableView) : base(tableView)
        {
        }

        public override Type GetCellType(object forDataContext)
        {
            if (forDataContext is FinanceOperation)
                return typeof(FinanceOperationCell);

            if (forDataContext is EmptyFinanceOperationSection)
                return typeof(FinanceOperationSectionHeaderLikeEmptyCell);

            if (forDataContext is HeaderFinanceOperationSection)
                return typeof(FinanceOperationHeaderLikeCell);
            
            if (forDataContext is ContactDetailsHeaderSection)
                return typeof(ContactDetailsHeaderCell);
                
            if (forDataContext is ContactDetailsFooterDebtsStatisticsSection)
                return typeof(ContactDetailsDebtsStatisticsCell); 
            
            if (forDataContext is ContactDetailsFooterLoansStatisticsSection)
                return typeof(ContactDetailsLoanStatisticsCell); 
            
            throw new InvalidOperationException();
        }

        public override object GetItemAt(IEnumerable itemsSource, int groupIndex, int itemIndex)
        {
            var group = GetGroupItemAt(itemsSource, groupIndex) as DetailsSection;

            if (group is DetailsFinanceOperationsSection operationsSection)
            {
                itemIndex--; // handle "empty notes" state
                
                if (itemIndex < 0 && !operationsSection.HasAnyOperation)
                    return new EmptyFinanceOperationSection(operationsSection);
                
                if (itemIndex < 0 && operationsSection.HasAnyOperation)
                    return new HeaderFinanceOperationSection(operationsSection);

                return operationsSection.Operations.ElementAt(itemIndex);
            }

            return group;
        }

        public override object GetGroupItemAt(IEnumerable itemsSource, int groupIndex) => (itemsSource.ElementAt(groupIndex) as DetailsSection);

        public override int GetNumberOfItemsInGroup(IEnumerable itemsSource, int section)
        {
            var groupItem = GetGroupItemAt(itemsSource, section);

            if (groupItem is DetailsFinanceOperationsSection detailsFinanceOperationsSection)
                return detailsFinanceOperationsSection.ItemsCount + 1;
            
            if (groupItem is ContactDetailsHeaderSection || 
                groupItem is ContactDetailsFooterDebtsStatisticsSection ||
                groupItem is ContactDetailsFooterLoansStatisticsSection)
                return 1;

            return 0;
        }

        protected override Type GetGroupHeaderCellType(object fromDataContext) => null;
 

        protected override Type GetGroupFooterCellType(object fromDataContext) => null;
    }
}