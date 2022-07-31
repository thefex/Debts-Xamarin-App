using System;
using System.Collections;
using Debts.Data;
using Debts.iOS.Cells;
using Debts.iOS.Cells.Budget;
using Debts.iOS.Config;
using Debts.Model;
using Debts.ViewModel.Budget;
using Debts.ViewModel.FinancesViewModel;
using MvvmCross.Binding.Extensions;
using UIKit;

namespace Debts.iOS.Utilities.TableView.Grouped
{
    public class BudgetListTemplateSelector : MvxGroupedTemplateSelector
    {
        private readonly Func<BudgetListViewModel> _viewModel;

        public BudgetListTemplateSelector(UITableView tableView, Func<BudgetListViewModel> viewModel) : base(tableView)
        {
            _viewModel = viewModel;
        }

        public override Type GetCellType(object forDataContext)
        {
            return typeof(BudgetCell);
        }

        public override object GetItemAt(IEnumerable itemsSource, int groupIndex, int itemIndex)
        {
            var group = GetGroupItemAt(itemsSource, groupIndex) as BudgetItemGroup;
            return group.GroupChilds.ElementAt(itemIndex);
        }

        public override object GetGroupItemAt(IEnumerable itemsSource, int groupIndex) => (itemsSource.ElementAt(groupIndex) as BudgetItemGroup);

        public override int GetNumberOfItemsInGroup(IEnumerable itemsSource, int section)
        {
            var groupItem = GetGroupItemAt(itemsSource, section);
            return (groupItem as BudgetItemGroup)?.GroupChilds?.Count() ?? 0;
        }

        public override UIContextualAction[] GetTrailingSwipeContextualActions(object forDataContext)
        {
            if (forDataContext is BudgetItem budgetItem)
            {
                var deleteAction = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Destructive,
                    "Delete".ToUpperInvariant(),
                    (action, view, handler) =>
                    {
                        handler.Invoke(true);
                        _viewModel().Delete.Execute(budgetItem);
                    });

                deleteAction.Image = UIImage.GetSystemImage("trash.fill");
                deleteAction.BackgroundColor = AppColors.AppRed;
                
                return new UIContextualAction[]
                {
                    deleteAction
                };
            }
            
            return base.GetTrailingSwipeContextualActions(forDataContext);
        }

        protected override Type GetGroupHeaderCellType(object fromDataContext) => typeof(BudgetGroupCell);
        
        protected override Type GetGroupFooterCellType(object fromDataContext) => null;
    }
}