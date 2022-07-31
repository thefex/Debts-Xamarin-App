using System;
using System.Collections;
using CloudKit;
using Debts.Data;
using Debts.iOS.Cells;
using Debts.iOS.Config;
using Debts.Model;
using Debts.ViewModel.FinancesViewModel;
using MvvmCross.Binding.Extensions;
using UIKit;

namespace Debts.iOS.Utilities.TableView.Grouped
{
    public class FinanceListTemplateSelector : MvxGroupedTemplateSelector
    {
        private readonly Func<BaseFinancesViewModel> _viewModel;

        public FinanceListTemplateSelector(UITableView tableView, Func<BaseFinancesViewModel> viewModel) : base(tableView)
        {
            _viewModel = viewModel;
        }

        public override Type GetCellType(object forDataContext)
        {
            return typeof(FinanceOperationCell);
        }

        public override object GetItemAt(IEnumerable itemsSource, int groupIndex, int itemIndex)
        {
            var group = GetGroupItemAt(itemsSource, groupIndex) as FinancesOperationGroup;
            return group.GroupChilds.ElementAt(itemIndex);
        }

        public override object GetGroupItemAt(IEnumerable itemsSource, int groupIndex) => (itemsSource.ElementAt(groupIndex) as FinancesOperationGroup);

        public override int GetNumberOfItemsInGroup(IEnumerable itemsSource, int section)
        {
            var groupItem = GetGroupItemAt(itemsSource, section);
            return (groupItem as FinancesOperationGroup)?.GroupChilds?.Count() ?? 0;
        }

        public override UIContextualAction[] GetLeadingSwipeContextualActions(object forDataContext)
        {
            if (forDataContext is FinanceOperation operation && !operation.IsPaid)
            {
                var payOffAction = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Normal,
                    "Mark as paid",
                    (action, view, handler) =>
                    {
                        handler.Invoke(true);
                        _viewModel().Finalize.Execute(operation);
                    });
                payOffAction.Image = UIImage.GetSystemImage("checkmark.circle.fill");
                payOffAction.BackgroundColor = AppColors.BlueSwipeColor; 
                return new UIContextualAction[]
                {
                    payOffAction
                };
            }
            
            return base.GetLeadingSwipeContextualActions(forDataContext);
        }

        public override UIContextualAction[] GetTrailingSwipeContextualActions(object forDataContext)
        {
            if (forDataContext is FinanceOperation operation)
            {
                var deleteAction = UIContextualAction.FromContextualActionStyle(UIContextualActionStyle.Destructive,
                    "Delete".ToUpperInvariant(),
                    (action, view, handler) =>
                    {
                        handler.Invoke(true);
                        _viewModel().Delete.Execute(operation);
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

        protected override Type GetGroupHeaderCellType(object fromDataContext) => typeof(FinancesOperationGroupCell);
        
        protected override Type GetGroupFooterCellType(object fromDataContext) => null;
    }
}