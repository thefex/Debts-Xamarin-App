using System;
using System.Collections;
using Debts.Data;
using Debts.iOS.Cells;
using Debts.iOS.Core.Data;
using Debts.iOS.Core.TableView.Sections;
using Debts.Model.Sections;
using MvvmCross.Binding.Extensions;
using UIKit;

namespace Debts.iOS.Utilities.TableView.Grouped
{
    public class FinanceDetailsSectionTemplateSelector : MvxGroupedTemplateSelector
    {
        public FinanceDetailsSectionTemplateSelector(UITableView tableView) : base(tableView)
        {
        }

        public override Type GetCellType(object forDataContext)
        {
            if (forDataContext is Note)
                return typeof(NoteCell);

            if (forDataContext is EmptyNotesSection)
                return typeof(HeaderLikeEmptyNoteCell);

            if (forDataContext is HeaderNotesSection)
                return typeof(HeaderLikeNoteCell);
            
            if (forDataContext is FinanceDetailsHeaderSection)
                return typeof(FinanceDetailsHeaderCell);
                
            if (forDataContext is FinanceDetailsFooterSection)
                return typeof(FinanceDetailsFooterCell); 
            
            throw new InvalidOperationException();
        }

        public override object GetItemAt(IEnumerable itemsSource, int groupIndex, int itemIndex)
        {
            var group = GetGroupItemAt(itemsSource, groupIndex) as DetailsSection;

            if (group is DetailsNotesSection notesSection)
            {
                itemIndex--; // handle "empty notes" state
                
                if (itemIndex < 0 && !notesSection.HasAnyNote)
                    return new EmptyNotesSection(notesSection);
                
                if (itemIndex < 0 && notesSection.HasAnyNote)
                    return new HeaderNotesSection(notesSection);

                return notesSection.Notes.ElementAt(itemIndex);
            }

            return group;
        }

        public override object GetGroupItemAt(IEnumerable itemsSource, int groupIndex) => (itemsSource.ElementAt(groupIndex) as DetailsSection);

        public override int GetNumberOfItemsInGroup(IEnumerable itemsSource, int section)
        {
            var groupItem = GetGroupItemAt(itemsSource, section);

            if (groupItem is DetailsNotesSection detailsNotesSection)
                return detailsNotesSection.ItemsCount + 1;
            
            if (groupItem is FinanceDetailsHeaderSection || groupItem is FinanceDetailsFooterSection)
                return 1;

            return 0;
        }

        protected override Type GetGroupHeaderCellType(object fromDataContext) => null;
 

        protected override Type GetGroupFooterCellType(object fromDataContext) => null;
    }
}